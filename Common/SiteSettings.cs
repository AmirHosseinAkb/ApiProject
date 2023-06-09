﻿namespace Common
{
    public class SiteSettings
    {
        public string ElmahPath { get; set; }
        public JwtSettings JwtSettings { get; set; }
    }

    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationDays { get; set; }
        public int NotBeforeMinutes { get; set; }
        public string SecretKey { get; set; }
        public string EncryptKey { get; set; }
    }
}
