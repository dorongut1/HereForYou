-- ============================================
-- Parental Presence App - Database Schema
-- SQLite Database for Local Storage
-- ============================================

-- ============================================
-- Table: detection_events
-- Purpose: כל אירוע זיהוי של מילת מפתח
-- ============================================
CREATE TABLE IF NOT EXISTS detection_events (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    keyword TEXT NOT NULL,                    -- "אמא", "אבא", שם הילד
    confidence REAL NOT NULL,                 -- 0.0 - 1.0
    was_responded_to BOOLEAN NOT NULL DEFAULT 0,
    response_time_seconds INTEGER,            -- כמה שניות לקח להגיב (NULL = לא הגיב)
    was_part_of_alert BOOLEAN DEFAULT 0,      -- האם היה חלק מהתראה (3+ זיהויים)
    alert_id INTEGER,                         -- קישור ל-alert אם היה
    context TEXT,                             -- JSON עם context נוסף
    FOREIGN KEY (alert_id) REFERENCES alerts(id)
);

-- אינדקסים לביצועים
CREATE INDEX IF NOT EXISTS idx_detection_timestamp ON detection_events(timestamp);
CREATE INDEX IF NOT EXISTS idx_detection_keyword ON detection_events(keyword);
CREATE INDEX IF NOT EXISTS idx_detection_responded ON detection_events(was_responded_to);

-- ============================================
-- Table: alerts
-- Purpose: התראות שנשלחו להורה (3+ זיהויים)
-- ============================================
CREATE TABLE IF NOT EXISTS alerts (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    keyword TEXT NOT NULL,
    detection_count INTEGER NOT NULL,         -- כמה זיהויים היו
    alert_level TEXT NOT NULL,                -- 'Info', 'Warning', 'Critical'
    was_acknowledged BOOLEAN NOT NULL DEFAULT 0,
    acknowledged_at DATETIME,
    acknowledgment_type TEXT,                 -- 'handled', 'snoozed', 'dismissed'
    time_window_seconds INTEGER DEFAULT 30    -- חלון הזמן שבו היו הזיהויים
);

CREATE INDEX IF NOT EXISTS idx_alert_created ON alerts(created_at);
CREATE INDEX IF NOT EXISTS idx_alert_acknowledged ON alerts(was_acknowledged);

-- ============================================
-- Table: screen_time_sessions
-- Purpose: מעקב אחר זמן מסך
-- ============================================
CREATE TABLE IF NOT EXISTS screen_time_sessions (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    start_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    end_time DATETIME,
    duration_seconds INTEGER,                 -- מחושב: end_time - start_time
    app_name TEXT DEFAULT 'General',          -- שם האפליקציה שהייתה פעילה
    was_interrupted BOOLEAN DEFAULT 0,        -- האם היה זיהוי/התראה בזמן הסשן
    interruption_count INTEGER DEFAULT 0,     -- כמה פעמים היה זיהוי
    device_type TEXT DEFAULT 'Mobile'         -- 'Mobile', 'Desktop', 'Tablet'
);

CREATE INDEX IF NOT EXISTS idx_session_start ON screen_time_sessions(start_time);
CREATE INDEX IF NOT EXISTS idx_session_date ON screen_time_sessions(date(start_time));

-- ============================================
-- Table: daily_summaries
-- Purpose: סיכומים יומיים מחושבים (לביצועים)
-- ============================================
CREATE TABLE IF NOT EXISTS daily_summaries (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    date DATE NOT NULL UNIQUE,
    total_screen_time_seconds INTEGER NOT NULL DEFAULT 0,
    total_detections INTEGER NOT NULL DEFAULT 0,
    total_alerts INTEGER NOT NULL DEFAULT 0,
    responded_detections INTEGER NOT NULL DEFAULT 0,
    average_response_time_seconds INTEGER,
    presence_score REAL,                      -- 0-100
    longest_screen_session_seconds INTEGER,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_daily_date ON daily_summaries(date);

-- ============================================
-- Table: user_settings
-- Purpose: הגדרות משתמש
-- ============================================
CREATE TABLE IF NOT EXISTS user_settings (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL,
    value_type TEXT NOT NULL,                 -- 'string', 'int', 'bool', 'json'
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- ערכי ברירת מחדל
INSERT OR IGNORE INTO user_settings (key, value, value_type) VALUES
    ('keywords', '["אמא","אבא"]', 'json'),
    ('detection_threshold', '3', 'int'),
    ('threshold_window_seconds', '30', 'int'),
    ('is_monitoring_enabled', 'false', 'bool'),
    ('alert_level_default', 'Warning', 'string'),
    ('enable_overlay_alerts', 'true', 'bool'),
    ('enable_sound_alerts', 'true', 'bool'),
    ('enable_vibration', 'true', 'bool'),
    ('confidence_threshold', '0.7', 'float'),
    ('user_name', '', 'string'),
    ('family_member_names', '[]', 'json');

-- ============================================
-- Table: keyword_profiles
-- Purpose: פרופילי מילות מפתח מותאמים אישית
-- ============================================
CREATE TABLE IF NOT EXISTS keyword_profiles (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    keyword TEXT NOT NULL UNIQUE,
    display_name TEXT NOT NULL,               -- "אמא", "אבא", "דורון"
    model_path TEXT,                          -- נתיב למודל Picovoice מותאם
    sensitivity REAL DEFAULT 0.7,             -- 0.0 - 1.0
    is_enabled BOOLEAN DEFAULT 1,
    voice_sample_path TEXT,                   -- נתיב לדוגמת קול (אם יש)
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- מילות מפתח ברירת מחדל
INSERT OR IGNORE INTO keyword_profiles (keyword, display_name) VALUES
    ('ima', 'אמא'),
    ('abba', 'אבא');

-- ============================================
-- Table: analytics_events
-- Purpose: אירועי אנליטיקס כלליים
-- ============================================
CREATE TABLE IF NOT EXISTS analytics_events (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    event_type TEXT NOT NULL,                 -- 'app_opened', 'monitoring_started', etc.
    event_data TEXT,                          -- JSON עם נתונים נוספים
    timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_analytics_type ON analytics_events(event_type);
CREATE INDEX IF NOT EXISTS idx_analytics_timestamp ON analytics_events(timestamp);

-- ============================================
-- Views: תצוגות מחושבות
-- ============================================

-- תצוגה: סטטיסטיקות יומיות (real-time)
CREATE VIEW IF NOT EXISTS v_daily_stats AS
SELECT 
    DATE(start_time) as date,
    COUNT(DISTINCT id) as total_sessions,
    SUM(duration_seconds) as total_screen_time_seconds,
    AVG(duration_seconds) as avg_session_duration_seconds,
    MAX(duration_seconds) as longest_session_seconds,
    SUM(CASE WHEN was_interrupted = 1 THEN 1 ELSE 0 END) as interrupted_sessions
FROM screen_time_sessions
WHERE end_time IS NOT NULL
GROUP BY DATE(start_time);

-- תצוגה: סטטיסטיקות זיהוי יומיות
CREATE VIEW IF NOT EXISTS v_detection_stats AS
SELECT 
    DATE(timestamp) as date,
    keyword,
    COUNT(*) as total_detections,
    SUM(CASE WHEN was_responded_to = 1 THEN 1 ELSE 0 END) as responded_count,
    AVG(CASE WHEN response_time_seconds IS NOT NULL THEN response_time_seconds END) as avg_response_time_seconds,
    MIN(response_time_seconds) as fastest_response_seconds,
    MAX(response_time_seconds) as slowest_response_seconds
FROM detection_events
GROUP BY DATE(timestamp), keyword;

-- תצוגה: סיכום התראות
CREATE VIEW IF NOT EXISTS v_alert_summary AS
SELECT 
    DATE(created_at) as date,
    alert_level,
    COUNT(*) as total_alerts,
    SUM(CASE WHEN was_acknowledged = 1 THEN 1 ELSE 0 END) as acknowledged_count,
    AVG(CASE 
        WHEN acknowledged_at IS NOT NULL 
        THEN (julianday(acknowledged_at) - julianday(created_at)) * 86400 
    END) as avg_acknowledgment_time_seconds
FROM alerts
GROUP BY DATE(created_at), alert_level;

-- ============================================
-- Triggers: אוטומציה
-- ============================================

-- Trigger: עדכון duration_seconds אוטומטית
CREATE TRIGGER IF NOT EXISTS trg_update_session_duration
AFTER UPDATE OF end_time ON screen_time_sessions
WHEN NEW.end_time IS NOT NULL AND OLD.end_time IS NULL
BEGIN
    UPDATE screen_time_sessions
    SET duration_seconds = (julianday(NEW.end_time) - julianday(NEW.start_time)) * 86400
    WHERE id = NEW.id;
END;

-- Trigger: עדכון daily_summaries אוטומטית כאשר מסתיים סשן
CREATE TRIGGER IF NOT EXISTS trg_update_daily_summary_on_session
AFTER UPDATE OF end_time ON screen_time_sessions
WHEN NEW.end_time IS NOT NULL AND OLD.end_time IS NULL
BEGIN
    INSERT INTO daily_summaries (date, total_screen_time_seconds, updated_at)
    VALUES (DATE(NEW.start_time), NEW.duration_seconds, CURRENT_TIMESTAMP)
    ON CONFLICT(date) DO UPDATE SET
        total_screen_time_seconds = total_screen_time_seconds + NEW.duration_seconds,
        updated_at = CURRENT_TIMESTAMP;
END;

-- Trigger: עדכון daily_summaries כשיש זיהוי חדש
CREATE TRIGGER IF NOT EXISTS trg_update_daily_summary_on_detection
AFTER INSERT ON detection_events
BEGIN
    INSERT INTO daily_summaries (date, total_detections, updated_at)
    VALUES (DATE(NEW.timestamp), 1, CURRENT_TIMESTAMP)
    ON CONFLICT(date) DO UPDATE SET
        total_detections = total_detections + 1,
        responded_detections = responded_detections + 
            CASE WHEN NEW.was_responded_to = 1 THEN 1 ELSE 0 END,
        updated_at = CURRENT_TIMESTAMP;
END;

-- Trigger: עדכון updated_at אוטומטית
CREATE TRIGGER IF NOT EXISTS trg_update_settings_timestamp
AFTER UPDATE ON user_settings
BEGIN
    UPDATE user_settings SET updated_at = CURRENT_TIMESTAMP WHERE key = NEW.key;
END;

-- ============================================
-- Initial Data: נתונים לדוגמה (רק ל-development)
-- ============================================
-- אלה יימחקו בגרסת production

-- (אם רוצים לטעון נתונים לבדיקה, אפשר כאן)

-- ============================================
-- Maintenance Queries
-- ============================================

-- ניקוי אירועים ישנים (מעל 90 יום)
-- יש להריץ periodically
-- DELETE FROM detection_events WHERE timestamp < datetime('now', '-90 days');
-- DELETE FROM analytics_events WHERE timestamp < datetime('now', '-90 days');

-- ============================================
-- Schema Version Tracking
-- ============================================
CREATE TABLE IF NOT EXISTS schema_version (
    version INTEGER PRIMARY KEY,
    applied_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    description TEXT
);

INSERT INTO schema_version (version, description) VALUES 
    (1, 'Initial schema with core tables');
