using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace SGame.Core
{
    /// <summary>
    /// Local-only account store + active session.
    ///
    /// Accounts live in PlayerPrefs as JSON. Passwords are stored as a SHA-256
    /// hash with a per-account salt so a casual inspection of the prefs file
    /// can't recover the plaintext. This is NOT a security boundary — the
    /// device owner can always wipe / replace the prefs, and we have no
    /// backend yet — but it's the right shape so we can plug in a real
    /// service later (Unity Authentication, Firebase, custom REST) without
    /// changing call sites.
    /// </summary>
    public static class UserSession
    {
        private const string AccountsKey = "sgame.accounts.v1";
        private const string ActiveUserKey = "sgame.session.activeUser";
        private const string GuestUser = "guest";

        public enum AuthResult { Ok, EmailTaken, InvalidCredentials, WeakPassword, BadEmail, MissingFields }

        [Serializable]
        public class Account
        {
            public string displayName;
            public string email;     // lower-cased, trimmed
            public string salt;      // hex
            public string passwordHash; // hex(SHA256(salt + password))
            public long createdAtUtcTicks;
        }

        [Serializable]
        private class AccountStore
        {
            public List<Account> accounts = new List<Account>();
        }

        // ── Public API ──────────────────────────────────────────────────────

        /// <summary>The email (or "guest") of the currently signed-in user, or null if none.</summary>
        public static string ActiveUserEmail
        {
            get
            {
                var s = PlayerPrefs.GetString(ActiveUserKey, "");
                return string.IsNullOrEmpty(s) ? null : s;
            }
        }

        public static bool IsLoggedIn => !string.IsNullOrEmpty(ActiveUserEmail);
        public static bool IsGuest => ActiveUserEmail == GuestUser;

        /// <summary>Display name to show in the HUD ("Guest" for guest sessions).</summary>
        public static string ActiveDisplayName
        {
            get
            {
                var email = ActiveUserEmail;
                if (string.IsNullOrEmpty(email)) return null;
                if (email == GuestUser) return "Guest";
                var acc = FindAccount(email);
                return acc?.displayName ?? email;
            }
        }

        public static AuthResult Register(string displayName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(displayName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
                return AuthResult.MissingFields;

            email = email.Trim().ToLowerInvariant();
            if (!IsEmailShaped(email)) return AuthResult.BadEmail;
            if (password.Length < 6) return AuthResult.WeakPassword;

            var store = LoadStore();
            for (int i = 0; i < store.accounts.Count; i++)
            {
                if (store.accounts[i].email == email) return AuthResult.EmailTaken;
            }

            var salt = NewSalt();
            store.accounts.Add(new Account
            {
                displayName = displayName.Trim(),
                email = email,
                salt = salt,
                passwordHash = HashPassword(salt, password),
                createdAtUtcTicks = DateTime.UtcNow.Ticks,
            });
            SaveStore(store);
            SetActive(email);
            return AuthResult.Ok;
        }

        public static AuthResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return AuthResult.MissingFields;

            email = email.Trim().ToLowerInvariant();
            var acc = FindAccount(email);
            if (acc == null) return AuthResult.InvalidCredentials;
            if (HashPassword(acc.salt, password) != acc.passwordHash)
                return AuthResult.InvalidCredentials;

            SetActive(email);
            return AuthResult.Ok;
        }

        public static void LoginAsGuest()
        {
            SetActive(GuestUser);
        }

        public static void Logout()
        {
            PlayerPrefs.DeleteKey(ActiveUserKey);
            PlayerPrefs.Save();
        }

        public static bool AccountExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return FindAccount(email.Trim().ToLowerInvariant()) != null;
        }

        // ── Internals ───────────────────────────────────────────────────────

        private static void SetActive(string email)
        {
            PlayerPrefs.SetString(ActiveUserKey, email);
            PlayerPrefs.Save();
        }

        private static Account FindAccount(string email)
        {
            var store = LoadStore();
            for (int i = 0; i < store.accounts.Count; i++)
            {
                if (store.accounts[i].email == email) return store.accounts[i];
            }
            return null;
        }

        private static AccountStore LoadStore()
        {
            var json = PlayerPrefs.GetString(AccountsKey, "");
            if (string.IsNullOrEmpty(json)) return new AccountStore();
            try { return JsonUtility.FromJson<AccountStore>(json) ?? new AccountStore(); }
            catch { return new AccountStore(); }
        }

        private static void SaveStore(AccountStore store)
        {
            PlayerPrefs.SetString(AccountsKey, JsonUtility.ToJson(store));
            PlayerPrefs.Save();
        }

        // Returns 16 random bytes as a 32-char hex string.
        private static string NewSalt()
        {
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(bytes);
            return ToHex(bytes);
        }

        private static string HashPassword(string salt, string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(salt + ":" + password));
                return ToHex(bytes);
            }
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++) sb.Append(bytes[i].ToString("x2"));
            return sb.ToString();
        }

        // Permissive enough for local sign-up: requires "x@y.z" shape.
        // We deliberately don't enforce a stricter RFC-5321 regex because
        // there's no SMTP delivery — the field is really just a unique ID.
        private static bool IsEmailShaped(string s)
        {
            int at = s.IndexOf('@');
            int dot = s.LastIndexOf('.');
            return at > 0 && dot > at + 1 && dot < s.Length - 1;
        }
    }
}
