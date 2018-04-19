namespace HRP.Core.Model
{
    using System;

    public class AppSettingsOptions
    {
        public int SessionTimeout { set; get; }

        public bool EnableEncrypt { get; set; }

        public string Domain { get; set; }

        public DateTime LaunchMonth { get; set; }
    }
}