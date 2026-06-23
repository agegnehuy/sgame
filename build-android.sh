#!/usr/bin/env bash
#
# Build an Android APK for SafeStepsAddis without opening the Unity Editor.
# Equivalent to clicking "SGame → Build Android APK" inside the editor.
#
# Usage:
#   ./build-android.sh                 # build APK
#   ./build-android.sh install         # build, then `adb install -r` to phone
#   ./build-android.sh run             # build, install, then launch the app
#
# Requires:
#   - Unity 6000.3.16f1 (matching ProjectSettings/ProjectVersion.txt)
#   - Android Build Support module installed in Unity Hub
#   - For install/run: adb on PATH and a phone with USB debugging on
#
set -euo pipefail

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
UNITY_VERSION="$(grep '^m_EditorVersion:' "$PROJECT_DIR/ProjectSettings/ProjectVersion.txt" | awk '{print $2}')"
UNITY_EXE="$HOME/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity"

if [ ! -x "$UNITY_EXE" ]; then
    echo "Unity editor not found at: $UNITY_EXE"
    echo "Open Unity Hub and install version $UNITY_VERSION with the Android Build Support module."
    exit 1
fi

LOG_FILE="$PROJECT_DIR/Builds/build.log"
APK_PATH="$PROJECT_DIR/Builds/SafeStepsAddis.apk"
mkdir -p "$PROJECT_DIR/Builds"

echo "==> Building APK with Unity $UNITY_VERSION (log: $LOG_FILE)"
"$UNITY_EXE" \
    -batchmode \
    -nographics \
    -quit \
    -projectPath "$PROJECT_DIR" \
    -buildTarget Android \
    -executeMethod SGame.EditorTools.AndroidBuilder.PerformBuild \
    -logFile "$LOG_FILE"

if [ ! -f "$APK_PATH" ]; then
    echo "Build did not produce an APK. Inspect $LOG_FILE."
    tail -n 40 "$LOG_FILE" || true
    exit 1
fi

SIZE=$(du -h "$APK_PATH" | awk '{print $1}')
echo "==> APK ready: $APK_PATH ($SIZE)"

case "${1:-}" in
    install|run)
        if ! command -v adb >/dev/null 2>&1; then
            ADB_BUNDLED="$HOME/Unity/Hub/Editor/$UNITY_VERSION/Editor/Data/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb"
            if [ -x "$ADB_BUNDLED" ]; then
                ADB="$ADB_BUNDLED"
            else
                echo "adb not found. Install android-tools-adb or add Unity's platform-tools to PATH."
                exit 1
            fi
        else
            ADB="adb"
        fi
        echo "==> Installing on connected device"
        "$ADB" install -r "$APK_PATH"
        if [ "$1" = "run" ]; then
            PACKAGE=$(grep -E '^\s+Android:\s+com\.' "$PROJECT_DIR/ProjectSettings/ProjectSettings.asset" | head -1 | awk '{print $2}')
            echo "==> Launching $PACKAGE"
            "$ADB" shell monkey -p "$PACKAGE" -c android.intent.category.LAUNCHER 1
        fi
        ;;
esac
