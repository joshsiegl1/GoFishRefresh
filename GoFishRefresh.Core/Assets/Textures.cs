#region Using Statments
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion 

public static class Textures
{
    public static Texture2D CardBack;
    //This is used for the background of the hands screen
    public static Texture2D pixel;

    public static Texture2D background; 
    public static Texture2D ShowHandsButton;
    #region Card Textures
    public static Texture2D AS;
    public static Texture2D AC; 
    public static Texture2D AD;
    public static Texture2D AH;
    public static Texture2D TwoS;
    public static Texture2D TwoC;
    public static Texture2D TwoD;
    public static Texture2D TwoH;
    public static Texture2D ThreeS;
    public static Texture2D ThreeC;
    public static Texture2D ThreeD;
    public static Texture2D ThreeH;
    public static Texture2D FourS;
    public static Texture2D FourC;
    public static Texture2D FourD;
    public static Texture2D FourH;
    public static Texture2D FiveS;
    public static Texture2D FiveC;
    public static Texture2D FiveD;
    public static Texture2D FiveH;
    public static Texture2D SixS;
    public static Texture2D SixC;
    public static Texture2D SixD;
    public static Texture2D SixH;
    public static Texture2D SevenS;
    public static Texture2D SevenC;
    public static Texture2D SevenD;
    public static Texture2D SevenH;
    public static Texture2D EightS;
    public static Texture2D EightC;
    public static Texture2D EightD;
    public static Texture2D EightH;
    public static Texture2D NineS;
    public static Texture2D NineC;
    public static Texture2D NineD;
    public static Texture2D NineH;
    public static Texture2D TenS;
    public static Texture2D TenC;
    public static Texture2D TenD;
    public static Texture2D TenH;
    public static Texture2D JS;
    public static Texture2D JC;
    public static Texture2D JD;
    public static Texture2D JH;
    public static Texture2D QS;
    public static Texture2D QC;
    public static Texture2D QD;
    public static Texture2D QH;
    public static Texture2D KS;
    public static Texture2D KC;
    public static Texture2D KD;
    public static Texture2D KH;
    #endregion
    #region Get Card Texture Method
    public static Texture2D GetCardTexture(Card c)
    {
        switch (c.LoadString())
        {
            case "AS": return AS;
            case "AC": return AC;
            case "AD": return AD;
            case "AH": return AH;
            case "2S": return TwoS;
            case "2C": return TwoC;
            case "2D": return TwoD;
            case "2H": return TwoH;
            case "3S": return ThreeS;
            case "3C": return ThreeC;
            case "3D": return ThreeD;
            case "3H": return ThreeH;
            case "4S": return FourS;
            case "4C": return FourC;
            case "4D": return FourD;
            case "4H": return FourH;
            case "5S": return FiveS;
            case "5C": return FiveC;
            case "5D": return FiveD;
            case "5H": return FiveH;
            case "6S": return SixS;
            case "6C": return SixC;
            case "6D": return SixD;
            case "6H": return SixH;
            case "7S": return SevenS;
            case "7C": return SevenC;
            case "7D": return SevenD;
            case "7H": return SevenH;
            case "8S": return EightS;
            case "8C": return EightC;
            case "8D": return EightD;
            case "8H": return EightH;
            case "9S": return NineS;
            case "9C": return NineC;
            case "9D": return NineD;
            case "9H": return NineH;
            case "10S": return TenS;
            case "10C": return TenC;
            case "10D": return TenD;
            case "10H": return TenH;
            case "JS": return JS;
            case "JC": return JC;
            case "JD": return JD;
            case "JH": return JH;
            case "QS": return QS;
            case "QC": return QC;
            case "QD": return QD;
            case "QH": return QH;
            case "KS": return KS;
            case "KC": return KC;
            case "KD": return KD;
            case "KH": return KH;
            default: return CardBack;
        }
    }
    #endregion
    public static void LoadContent(ContentManager Content)
    {
        CardBack = Content.Load<Texture2D>("cardback");
        ShowHandsButton = Content.Load<Texture2D>("ShowHandsButton");
        pixel = Content.Load<Texture2D>("pixel");
        background = Content.Load<Texture2D>("background");
        #region Card Textures
        AS = Content.Load<Texture2D>("AS");
        AC = Content.Load<Texture2D>("AC");
        AD = Content.Load<Texture2D>("AD");
        AH = Content.Load<Texture2D>("AH");
        TwoS = Content.Load<Texture2D>("2S");
        TwoC = Content.Load<Texture2D>("2C");
        TwoD = Content.Load<Texture2D>("2D");
        TwoH = Content.Load<Texture2D>("2H");
        ThreeS = Content.Load<Texture2D>("3S");
        ThreeC = Content.Load<Texture2D>("3C");
        ThreeD = Content.Load<Texture2D>("3D");
        ThreeH = Content.Load<Texture2D>("3H");
        FourS = Content.Load<Texture2D>("4S");
        FourC = Content.Load<Texture2D>("4C");
        FourD = Content.Load<Texture2D>("4D");
        FourH = Content.Load<Texture2D>("4H");
        FiveS = Content.Load<Texture2D>("5S");
        FiveC = Content.Load<Texture2D>("5C");
        FiveD = Content.Load<Texture2D>("5D");
        FiveH = Content.Load<Texture2D>("5H");
        SixS = Content.Load<Texture2D>("6S");
        SixC = Content.Load<Texture2D>("6C");
        SixD = Content.Load<Texture2D>("6D");
        SixH = Content.Load<Texture2D>("6H");
        SevenS = Content.Load<Texture2D>("7S");
        SevenC = Content.Load<Texture2D>("7C");
        SevenD = Content.Load<Texture2D>("7D");
        SevenH = Content.Load<Texture2D>("7H");
        EightS = Content.Load<Texture2D>("8S");
        EightC = Content.Load<Texture2D>("8C");
        EightD = Content.Load<Texture2D>("8D");
        EightH = Content.Load<Texture2D>("8H");
        NineS = Content.Load<Texture2D>("9S");
        NineC = Content.Load<Texture2D>("9C");
        NineD = Content.Load<Texture2D>("9D");
        NineH = Content.Load<Texture2D>("9H");
        TenS = Content.Load<Texture2D>("10S");
        TenC = Content.Load<Texture2D>("10C");
        TenD = Content.Load<Texture2D>("10D");
        TenH = Content.Load<Texture2D>("10H");
        JS = Content.Load<Texture2D>("JS");
        JC = Content.Load<Texture2D>("JC");
        JD = Content.Load<Texture2D>("JD");
        JH = Content.Load<Texture2D>("JH");
        QS = Content.Load<Texture2D>("QS");
        QC = Content.Load<Texture2D>("QC");
        QD = Content.Load<Texture2D>("QD");
        QH = Content.Load<Texture2D>("QH");
        KS = Content.Load<Texture2D>("KS");
        KC = Content.Load<Texture2D>("KC");
        KD = Content.Load<Texture2D>("KD");
        KH = Content.Load<Texture2D>("KH");
        #endregion
    }
}