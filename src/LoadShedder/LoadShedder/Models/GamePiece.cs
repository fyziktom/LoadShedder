using LoadShedder.Common;
using System.Text.Json.Serialization;
using VEDriversLite.EntitiesBlocks.Entities;

namespace LoadShedder.Models
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
        One_Hundred = 100,
        One_Hundred_Twenty = 120,
        One_Hundred_Fifty = 150,
        One_Hundred_Eighty = 180,
        Two_Hundred_Twenty = 220,
        Two_Hundred_Seventy = 270,
        Three_Hundred_Thirty = 330,
        Four_Hundred_Seventy = 470,
        Five_Hundred_Sixty = 560,
        Eight_Hundred_Twenty = 820,
        One_Kilo = 1000,
        One_Kilo_Two = 1200,
        One_Kilo_Fifth = 1500,
        One_Kilo_Eight = 1800,
        Two_Kilo = 2000,
        Two_Kilo_Two = 2200,
        Three_Kilo_Three = 3300,
        Three_Kilo_Nine = 3900,
        Four_Kilo_Seven = 4700,
        Five_Kilo_Six = 5600,
        Six_Kilo_Eight = 5600,
        Eight_Kilo_Two = 8200,
        Ten_Kilo = 10000,
        Twenty_Two_Kilo = 22000,
        Forty_Seven_Kilo = 47000,
        Houndred_Kilo = 100000
    }


    public enum ResistorsCombos1
    {
        None = 0,
        Combo_1 = Resistors.One_Kilo,
        Combo_2 = Resistors.One_Kilo + Resistors.One_Hundred,
        Combo_3 = Resistors.One_Kilo + Resistors.One_Hundred_Twenty,
        Combo_4 = Resistors.One_Kilo + Resistors.One_Hundred_Fifty,
        Combo_5 = Resistors.One_Kilo + Resistors.One_Hundred_Eighty,
        Combo_6 = Resistors.One_Kilo + Resistors.Two_Hundred_Twenty,
        Combo_7 = Resistors.One_Kilo + Resistors.Two_Hundred_Seventy,
        Combo_8 = Resistors.One_Kilo + Resistors.Three_Hundred_Thirty,
        Combo_9 = Resistors.One_Kilo + Resistors.Four_Hundred_Seventy,
        Combo_10 = Resistors.One_Kilo_Two,
        Combo_11 = Resistors.One_Kilo_Two + Resistors.One_Hundred,
        Combo_12 = Resistors.One_Kilo_Fifth,
        Combo_13 = Resistors.One_Kilo_Eight,
        Combo_14 = Resistors.Two_Kilo,
        Combo_15 = Resistors.Two_Kilo + Resistors.One_Hundred_Eighty,
        Combo_16 = Resistors.Two_Kilo_Two,
        Combo_17 = Resistors.Two_Kilo_Two + Resistors.Three_Hundred_Thirty,
        Combo_18 = Resistors.Two_Kilo_Two + Resistors.Four_Hundred_Seventy,
        Combo_19 = Resistors.Three_Kilo_Three,
        Combo_20 = Resistors.Three_Kilo_Nine,
        Combo_21 = Resistors.Three_Kilo_Nine + Resistors.Four_Hundred_Seventy,
        Combo_22 = Resistors.Four_Kilo_Seven,
        Combo_23 = Resistors.Four_Kilo_Seven + Resistors.Three_Hundred_Thirty,
        Combo_24 = Resistors.Four_Kilo_Seven + Resistors.Five_Hundred_Sixty,
        Combo_25 = Resistors.Five_Kilo_Six,
        Combo_26 = Resistors.Five_Kilo_Six + Resistors.Four_Hundred_Seventy,
        Combo_27 = Resistors.Six_Kilo_Eight,
        Combo_28 = Resistors.Six_Kilo_Eight + Resistors.Three_Hundred_Thirty,
        Combo_29 = Resistors.Eight_Kilo_Two,
        Combo_30 = Resistors.Eight_Kilo_Two + Resistors.One_Kilo,
        Combo_31 = Resistors.Eight_Kilo_Two + Resistors.One_Kilo_Two,
        Combo_32 = Resistors.Ten_Kilo,
        Combo_33 = Resistors.Ten_Kilo + Resistors.One_Kilo,
        Combo_34 = Resistors.Ten_Kilo + Resistors.Two_Kilo,
        Combo_35 = Resistors.Ten_Kilo + Resistors.Three_Kilo_Three,
        Combo_36 = Resistors.Ten_Kilo + Resistors.Four_Kilo_Seven,
        Combo_37 = Resistors.Twenty_Two_Kilo,
        Combo_38 = Resistors.Twenty_Two_Kilo + Resistors.One_Kilo,
        Combo_39 = Resistors.Twenty_Two_Kilo + Resistors.Two_Kilo,
        Combo_40 = Resistors.Twenty_Two_Kilo + Resistors.Three_Kilo_Three,
        Combo_41 = Resistors.Twenty_Two_Kilo + Resistors.Four_Kilo_Seven,
        Combo_42 = Resistors.Twenty_Two_Kilo + Resistors.Six_Kilo_Eight,
        Combo_43 = Resistors.Forty_Seven_Kilo,
        Combo_44 = Resistors.Forty_Seven_Kilo - Resistors.Four_Kilo_Seven,
        Combo_45 = Resistors.Forty_Seven_Kilo - Resistors.Six_Kilo_Eight,
        Combo_46 = Resistors.Forty_Seven_Kilo - Resistors.Eight_Kilo_Two,
        Combo_47 = Resistors.Forty_Seven_Kilo - Resistors.Ten_Kilo,
        Combo_48 = Resistors.Forty_Seven_Kilo - Resistors.Twenty_Two_Kilo,
        Combo_49 = Resistors.Forty_Seven_Kilo - Resistors.Four_Kilo_Seven - Resistors.Two_Kilo,
        Combo_50 = Resistors.Forty_Seven_Kilo - Resistors.Four_Kilo_Seven - Resistors.Three_Kilo_Three
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

    /// <summary>
    /// Main Game Piece. Each contains some combination of the resistors
    /// </summary>
    public class GamePiece
    {
        /// <summary>
        /// ID of the GamePiece
        /// Maybe it will be obsolete, because the ResistorsCombo should create unique ID also
        /// Game should not have two same GamePieces
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Name of the GamePiece
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Description of the GamePiece
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Value of the energy of the virtual production or consumption by the GamePiece
        /// Value are in the kW - kilo Watt
        /// </summary>
        public double EnergyValue { get; set; } = 0;
        /// <summary>
        /// Helper flag for the UI. If it is true, the GamePiece is plugged to the circuit
        /// </summary>
        public bool IsPlugged { get; set; } = false;

        /// <summary>
        /// Voltage which shoud be measured to get this gamepiece.
        /// </summary>
        public double ExpectedVoltage { get; set; } = 0.0;
        /// <summary>
        /// Dedicated Channel number
        /// </summary>
        public int DedicatedChannelNumber { get; set; } = -1;

        /// <summary>
        /// Selected Resistors Combos
        /// </summary>
        public ResistorsCombos ResistorsCombo { get; set; } = ResistorsCombos.None;
        public ResistorsCombos1 ResistorsCombo1 { get; set; } = ResistorsCombos1.None;

        /// <summary>
        /// Type of the Game Piece item
        /// There are two now: Sources and Consumers. 
        /// Sources create energy and Consumers consume energy
        /// </summary>
        public GamePieceTypes GamePieceType { get; set; } = GamePieceTypes.Source;

        /// <summary>
        ///  It calculate the current through the resistor
        /// </summary>
        /// <returns>value in A</returns>
        public double GetGamePieceCurrent()
        {
            return (MainDataContext.ADCMainVoltage / 1000) / (MainDataContext.ADCMainDividingResistor + ((double)ResistorsCombo1));
        }

        /// <summary>
        /// Function calculate the voltage across the main resistor which is same for all game pieces
        /// </summary>
        /// <returns>value in mV</returns>
        public double GetMainResistorVoltage()
        {
            return (GetGamePieceCurrent() * 1000) * MainDataContext.ADCMainDividingResistor;
        }

        /// <summary>
        /// Function calculates the voltage across the GamePiece specific resistor
        /// </summary>
        /// <returns>value in mV</returns>
        public double GetGamePieceResistorVoltage()
        {
            return (GetGamePieceCurrent() * 1000) * (double)ResistorsCombo1;
        }

        /// <summary>
        /// Get Game piece based on input voltage
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns></returns>
        public static ResistorsCombos GetGamePieceTypeBasedOnVoltage(double voltage)
        {
            if (voltage <= 0.0)
                return ResistorsCombos.None;

            GamePiece tempGamePiece = new GamePiece();

            foreach (ResistorsCombos combo in Enum.GetValues(typeof(ResistorsCombos)))
            {
                if (combo != ResistorsCombos.None)
                {
                    tempGamePiece.ResistorsCombo = combo;
                    double expectedVoltage = tempGamePiece.GetGamePieceResistorVoltage();

                    if (Math.Abs(expectedVoltage - voltage) <= MainDataContext.ADCVoltageTolerance)
                        return combo;
                }
            }

            return ResistorsCombos.None;
            
        }

        /// <summary>
        /// Is match of the gamepiece piece based on input voltage
        /// </summary>
        /// <param name="voltage"></param>
        /// <returns></returns>
        public bool IsVoltageMatch(double voltage)
        {
            if (voltage <= 0.0)
                return false;

            if (Math.Abs(ExpectedVoltage - voltage) <= MainDataContext.ADCVoltageTolerance)
                return true;
            else
                return false;

        }

        /// <summary>
        /// Get list of resistors which creates specific combo
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public static List<Resistors> DecomposeComboIntoResistors(ResistorsCombos combo)
        {
            List<Resistors> result = new List<Resistors>();

            // Seřadit hodnoty rezistorů v sestupném pořadí
            var resistorsDescending = Enum.GetValues(typeof(Resistors))
                                          .Cast<Resistors>()
                                          .OrderByDescending(r => r)
                                          .ToList();

            int remainingComboValue = (int)combo;

            foreach (Resistors resistor in resistorsDescending)
            {
                if (remainingComboValue - (int)resistor >= 0)
                {
                    result.Add(resistor);
                    remainingComboValue -= (int)resistor;
                }
            }

            return result;
        }

        /// <summary>
        /// Get list of resistors which creates specific combo
        /// </summary>
        /// <param name="combo"></param>
        /// <returns></returns>
        public static List<Resistors> DecomposeCombo1IntoResistors(ResistorsCombos1 combo)
        {
            List<Resistors> result = new List<Resistors>();

            // Seřadit hodnoty rezistorů v sestupném pořadí
            var resistorsDescending = Enum.GetValues(typeof(Resistors))
                                          .Cast<Resistors>()
                                          .OrderByDescending(r => r)
                                          .ToList();

            int remainingComboValue = (int)combo;

            foreach (Resistors resistor in resistorsDescending)
            {
                if (remainingComboValue - (int)resistor >= 0)
                {
                    result.Add(resistor);
                    remainingComboValue -= (int)resistor;
                }
            }

            return result;
        }
    }
}
