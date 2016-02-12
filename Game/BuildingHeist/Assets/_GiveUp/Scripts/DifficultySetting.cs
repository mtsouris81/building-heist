

public class DifficultySetting
{
    public DifficultySetting(float defaultSetting, float easyRatio, float hardRatio)
    {
        DesignerSetting = defaultSetting;
        EasyRatio = easyRatio;
        HardRatio = hardRatio;
    }
    public float DesignerSetting { get; private set; }
    public float EasyRatio { get; set; }
    public float HardRatio { get; set; }

    public float GetValue(int difficulty)
    {
        switch (difficulty)
        {
            case 0:
                return DesignerSetting * EasyRatio;
            case 1:
                return DesignerSetting;
            case 2:
                return DesignerSetting * HardRatio;
            default:
                return DesignerSetting;
        }
    }
}
