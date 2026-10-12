using System;
using System.Collections.Generic;
using System.Text;

namespace hotelbooking.App
{
    public static class ApiConfig
    {
        public static string BaseUrl =>
            DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:8080/api"
                : "http://localhost:8080/api";
    }
}
