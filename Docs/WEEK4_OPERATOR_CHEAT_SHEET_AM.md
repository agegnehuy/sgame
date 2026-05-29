# የኦፕሬተር ፈጣን መመሪያ (1 ገጽ) - Week 4

ይህ ሰነድ በመስክ ስራ ጊዜ ፈጣን ማረጋገጫ ለማድረግ ይጠቅማል።

## 1) ፈጣን መጀመር (5 ደቂቃ)

- `HomeScene` (ወይም ለQA `HomeScene_QA`) ክፈት
- ከላይ `Profile`, `Coins`, `EN/AM` መታየቱን አረጋግጥ
- `Open Facilitator Panel` ጫን
- ትክክለኛ PIN አስገባ

ካልከፈተ:

- `requirePinToOpen = true` መሆኑን አረጋግጥ
- PIN የUI መስኮች በInspector መታሰራቸውን አረጋግጥ

## 2) የደህንነት ፈጣን ሙከራ (5 ደቂቃ)

- የተሳሳተ PIN 1 ጊዜ -> cooldown መታየት አለበት
- ብዙ ጊዜ የተሳሳተ PIN -> lockout መጀመር አለበት
- ጊዜው ካለፈ -> መቆጣጠሪያዎች መመለስ አለባቸው
- `Security:` ስቴት መለያ (Ready/Cooldown/Lockout) ትክክለኛ መሆን አለበት

## 3) የMaintenance ፈጣን ሙከራ (5 ደቂቃ)

- `Clear Analytics` -> የስኬት መልእክት
- `Clear Exports` -> የስኬት መልእክት
- `Clear PIN Audit` -> የስኬት መልእክት
- `Reset Profile` 1 ጊዜ -> ማረጋገጫ ማስጠንቀቂያ
- ፈጣን 2ኛ ጊዜ -> reset መፈጸም

## 4) EN/AM ሙከራ (2 ደቂቃ)

- EN -> AM -> EN ቀይር
- የfacilitator ጽሑፎች እና status መልእክቶች መቀየራቸውን አረጋግጥ
- የraw localization key እንዳይታይ አረጋግጥ

## 5) የሚመዘገቡ ነጥቦች (3 ደቂቃ)

- `WEEK4_SECURITY_REGRESSION_MATRIX.md` አዘምን
- `WEEK4_SECURITY_DAILY_LOG_TEMPLATE.md` አንድ ሪኮርድ አስቀምጥ
- ችግር ካለ -> `WEEK4_SECURITY_BUG_TEMPLATE.md` በመጠቀም `SEC-*` ፋይል ክፈት

## GO / NO-GO ህግ

- **GO**: Critical/High የደህንነት ችግሮች ካልነበሩ
- **NO-GO**: PIN access, lockout, audit flow ችግር ካለ

## ተጨማሪ ሰነዶች

- `WEEK4_SECURITY_DOCS_MAP.md`
- `WEEK4_DAY0_AND_FINAL_WIRING_COMBINED_CHECKLIST.md`
