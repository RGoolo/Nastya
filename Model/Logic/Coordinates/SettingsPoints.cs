namespace Model.Logic.Coordinates
{
    public class SettingsPoints
    {
        public Coordinate City { get; set; }
        public YandexSettings Yandex { get; } = new YandexSettings();
        public GoogleSettings Google { get; } = new GoogleSettings();
        public bool AddPicture { get; set; } = true;

        public class YandexSettings
        {
            public string Name { get; set; } = "[Y]";
            public string PointNameMe { get; set; } = "[Y from me]";
            public string PointName { get; set; } = "[Y point]";
            public bool LinkFor { get; set; } = true;
        }

        public class GoogleSettings
        {
            public string Name { get; set; } = "[G]";
            public string PointNameMe { get; set; } = "[G from me]";
            public string PointName { get; set; } = "[G point]";
            public bool LinkFor { get; set; } = true;
        }
    }
}