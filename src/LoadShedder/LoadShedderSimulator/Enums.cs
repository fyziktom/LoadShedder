
namespace LoadShedderSimulator
{


    public enum GamePieceTypes
    {
        Source,
        Consumer
    }

    /// <summary>
    /// Base line of configuration resistors for combos
    /// </summary>
    public enum Resistors
    {
        One_Kilo = 1000,
        Two_Kilo_Two = 2200,
        Four_Kilo_Seven = 4700,
        Eight_Kilo_Two = 8200,
        Twenty_Two_Kilo = 22000,
        Forty_Seven_Kilo = 47000,
        Houndred_Kilo = 100000
    }

    /// <summary>
    /// 63 possible combinations of resistors
    /// </summary>
    public enum ResistorsCombos
    {
        None = 0,
        Combo_1 = Resistors.One_Kilo,
        Combo_2 = Resistors.Two_Kilo_Two,
        Combo_3 = Resistors.One_Kilo + Resistors.Two_Kilo_Two,
        Combo_4 = Resistors.Four_Kilo_Seven,
        Combo_5 = Resistors.One_Kilo + Resistors.Four_Kilo_Seven,
        Combo_6 = Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven,
        Combo_7 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven,
        Combo_8 = Resistors.Eight_Kilo_Two,
        Combo_9 = Resistors.One_Kilo + Resistors.Eight_Kilo_Two,
        Combo_10 = Resistors.Two_Kilo_Two + Resistors.Eight_Kilo_Two,
        Combo_11 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Eight_Kilo_Two,
        Combo_12 = Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two,
        Combo_13 = Resistors.One_Kilo + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two,
        Combo_14 = Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two,
        Combo_15 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two,
        Combo_16 = Resistors.Twenty_Two_Kilo,
        Combo_17 = Resistors.One_Kilo + Resistors.Twenty_Two_Kilo,
        Combo_18 = Resistors.Two_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_19 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_20 = Resistors.Four_Kilo_Seven + Resistors.Twenty_Two_Kilo,
        Combo_21 = Resistors.One_Kilo + Resistors.Four_Kilo_Seven + Resistors.Twenty_Two_Kilo,
        Combo_22 = Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Twenty_Two_Kilo,
        Combo_23 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Twenty_Two_Kilo,
        Combo_24 = Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_25 = Resistors.One_Kilo + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_26 = Resistors.Two_Kilo_Two + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_27 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_28 = Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_29 = Resistors.One_Kilo + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_30 = Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_31 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo,
        Combo_32 = Resistors.Forty_Seven_Kilo,
        Combo_33 = Resistors.One_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_34 = Resistors.Two_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_35 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_36 = Resistors.Four_Kilo_Seven + Resistors.Forty_Seven_Kilo,
        Combo_37 = Resistors.One_Kilo + Resistors.Four_Kilo_Seven + Resistors.Forty_Seven_Kilo,
        Combo_38 = Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Forty_Seven_Kilo,
        Combo_39 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Forty_Seven_Kilo,
        Combo_40 = Resistors.Eight_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_41 = Resistors.One_Kilo + Resistors.Eight_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_42 = Resistors.Two_Kilo_Two + Resistors.Eight_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_43 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Eight_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_44 = Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_45 = Resistors.One_Kilo + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_46 = Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_47 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Forty_Seven_Kilo,
        Combo_48 = Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_49 = Resistors.One_Kilo + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_50 = Resistors.Two_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_51 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_52 = Resistors.Four_Kilo_Seven + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_53 = Resistors.One_Kilo + Resistors.Four_Kilo_Seven + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_54 = Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_55 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_56 = Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_57 = Resistors.One_Kilo + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_58 = Resistors.Two_Kilo_Two + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_59 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_60 = Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_61 = Resistors.One_Kilo + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_62 = Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo,
        Combo_63 = Resistors.One_Kilo + Resistors.Two_Kilo_Two + Resistors.Four_Kilo_Seven + Resistors.Eight_Kilo_Two + Resistors.Twenty_Two_Kilo + Resistors.Forty_Seven_Kilo

    }

}
