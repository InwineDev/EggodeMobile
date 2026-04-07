public static class SelectedMapState
{
    public static string PersistentMapPath;
    public static string ResourcesMapPath;
    public static string EmbeddedMapJson;

    public static void Clear()
    {
        PersistentMapPath = null;
        ResourcesMapPath = null;
        EmbeddedMapJson = null;
    }
}
