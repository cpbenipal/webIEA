namespace Flexpage.Code.Localization
{
    using System;
    using System.Text;
    using System.Threading;
    using Flexpage.Code.Helpers;

    public class LocalizedString
    {
        public enum NumbersIntoWordsLanguage { en, fr }

        /// <summary>
        /// Returns numbers into words in input language
        /// </summary>
        public static string NumbersIntoWords(long number, NumbersIntoWordsLanguage language)
        {
            switch (language)
            {
                case NumbersIntoWordsLanguage.en:
                    return NumbersIntoWordsEn.WriteNumber(number);
                case NumbersIntoWordsLanguage.fr:
                    return NumbersIntoWordsFr.Spell(number);
                default:
                    return NumbersIntoWordsEn.WriteNumber(number);
            }
        }

        /// <summary>
        /// Returns numbers into words in current thread language
        /// </summary>
        public static string NumbersIntoWords(long number)
        {
            string currentLang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            NumbersIntoWordsLanguage lang;
            if (Enum<NumbersIntoWordsLanguage>.TryParse(currentLang, out lang))
            {
                return LocalizedString.NumbersIntoWords(number, lang);
            }
            else
            {
                return LocalizedString.NumbersIntoWords(number, NumbersIntoWordsLanguage.en);
            }
        }

        /// <summary>
        /// Returns numbers into words in current thread language
        /// </summary>
        public static string NumbersIntoWords(decimal number)
        {
            return LocalizedString.NumbersIntoWords((long)number);
        }
    }

    /// <summary>
    /// Converts numbers into words (English)
    /// </summary>
    public class NumbersIntoWordsEn
    {
        private static string[] nums = 
        {
            "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
            "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Ninteen"
        };

        private static string[] tens = 
        {
            "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninty"
        };

        public static string WriteNumber(long Number, long Level = 0)
        {
            if ((Level == 0) & (Number == 0))
                return "Zero";
            if ((Level > 0) & (Number == 0))
                return "";
            if (Number < 0)
                return "Negative " + WriteNumber(Math.Abs(Number));

            if (Number < 20)
            {
                return nums[Number] + " ";
            }
            else if ((Number >= 20) && (Number <= 99))
            {
                return tens[Number / 10] + " " + WriteNumber(Number % 10, Level + 1);
            }
            else if ((Number >= 100) && (Number <= 999))
            {
                return nums[Number / 100] + " Hundred " + (((Number % 1000) == 0) ? "and " : "") +
                    WriteNumber(Number % 100, Level + 1);
            }
            else if ((Number >= 1000) && (Number <= 999999))
            {
                return WriteNumber(Number / 1000, Level + 1) + "Thousand " +
                    (Number % 1000 == 0 ? "" : " ") + WriteNumber(Number % 1000, Level + 1);
            }
            else if ((Number >= Math.Pow(10, 6)) && (Number < Math.Pow(10, 12)))
            {
                return WriteNumber((long)(Number / Math.Pow(10, 6)), Level + 1) + "Million " +
                    (Number % Math.Pow(10, 6) == 0 ? "" : " ") + WriteNumber((long)(Number % Math.Pow(10, 6)), Level + 1);
            }
            else if (Number >= Math.Pow(10, 12))
            {
                return WriteNumber((long)(Number / Math.Pow(10, 12)), Level + 1) + "Billion " +
                    (Number % Math.Pow(10, 12) == 0 ? "" : " ") + WriteNumber((long)(Number % Math.Pow(10, 12)), Level + 1);
            }
            return "";
        }
    }

    /// <summary>
    /// Converts numbers into words (French)
    /// </summary>
    public class NumbersIntoWordsFr
    {
        /// <summary>
        /// Genre grammatical.
        /// </summary>
        public enum Gender { Masculine = 0, Feminine = 1 }

        /// <summary>
        /// Nombre grammatical.
        /// </summary>
        public enum Number { Singular = 0, Plural = 1 }

        /// <summary>
        /// Adjectif numéral.
        /// </summary>
        public enum NumeralAdjective { Cardinal = 0, Ordinal = 1 }

        #region Private readonly fields

        private static readonly string[] _UNITSANDTENS;
        private static readonly string[] _HUNDREDS;
        private static readonly string[] _THOUSANDPOWERS;
        private static readonly string _MINUS;

        #endregion

        #region Constructor

        static NumbersIntoWordsFr()
        {
            _MINUS = "moins";

            // Les nombres jusqu'à cent ont beaucoup "d'exceptions" de nomenclature (23/99).
            // Il est plus simple de les stocker dans un tableau que de les générer dynamiquement :
            _UNITSANDTENS = new string[100]
            {
                string.Empty, "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf",
                "dix", "onze", "douze", "treize", "quatorze", "quinze", "seize", "dix-sept", "dix-huit", "dix-neuf",
                "vingt", "vingt et un", "vingt-deux", "vingt-trois", "vingt-quatre", "vingt-cinq", "vingt-six", "vingt-sept", "vingt-huit", "vingt-neuf",
                "trente", "trente et un", "trente-deux", "trente-trois", "trente-quatre", "trente-cinq", "trente-six", "trente-sept", "trente-huit", "trente-neuf",
                "quarante", "quarante et un", "quarante-deux", "quarante-trois", "quarante-quatre", "quarante-cinq", "quarante-six", "quarante-sept", "quarante-huit", "quarante-neuf",
                "cinquante", "cinquante et un", "cinquante-deux", "cinquante-trois", "cinquante-quatre", "cinquante-cinq", "cinquante-six", "cinquante-sept", "cinquante-huit", "cinquante-neuf",
                "soixante", "soixante et un", "soixante-deux", "soixante-trois", "soixante-quatre", "soixante-cinq", "soixante-six", "soixante-sept", "soixante-huit", "soixante-neuf",
                "soixante-dix", "soixante et onze", "soixante-douze", "soixante-treize", "soixante-quatorze", "soixante-quinze", "soixante-seize", "soixante-dix-sept", "soixante-dix-huit", "soixante-dix-neuf",
                "quatre-vingt", "quatre-vingt-un", "quatre-vingt-deux", "quatre-vingt-trois", "quatre-vingt-quatre", "quatre-vingt-cinq", "quatre-vingt-six", "quatre-vingt-sept", "quatre-vingt-huit", "quatre-vingt-neuf",
                "quatre-vingt-dix", "quatre-vingt-onze", "quatre-vingt-douze", "quatre-vingt-treize", "quatre-vingt-quatorze", "quatre-vingt-quinze", "quatre-vingt-seize", "quatre-vingt-dix-sept", "quatre-vingt-dix-huit", "quatre-vingt-dix-neuf"
            };
            _HUNDREDS = new string[10]
            {
                string.Empty, "cent", "deux cent", "trois cent", "quatre cent", "cinq cent", "six cent", "sept cent", "huit cent", "neuf cent"
            };
            _THOUSANDPOWERS = new string[7]
            {
                "trillion", "mille", "billion", "milliard", "million", "mille", string.Empty
            };

        }

        #endregion

        #region Private methods

        /// <summary>
        /// Accorder le mot "vingt".
        /// </summary>
        /// <returns>Un "s" si le nombre se termine par vingt et n'est pas suivi de "mille".
        /// Chaîne vide sinon.</returns>
        private static string MakeTwentyAgree(int value, int thousandPower, NumeralAdjective numeralAdjective)
        {
            // Vingt prend un "s" à la fin, si et seulement si :
            // - il fait partie d'un nombre cardinal.
            // - il n'est pas situé avant "mille".
            // - il se termine par quatre-vingts.
            if (numeralAdjective == NumeralAdjective.Cardinal &&
                thousandPower != 1 &&
                thousandPower != 5 &&
                value == 80)
                return "s";
            else
                return string.Empty;
        }

        /// <summary>
        /// Accorder le mot "cent".
        /// </summary>
        /// <returns>Un "s" si le nombre est un multiple de 100, strictement supérieur à 100 et n'est pas suivi de "mille".
        /// Chaîne vide sinon.</returns>
        private static string MakeHundredAgree(long hundreds, long tensAndUnits, int thousandPower, NumeralAdjective numeralAdjective)
        {
            // Cent prend un "s" à la fin, si et seulement si :
            // - il fait partie d'un nombre cardinal.
            // - il n'est pas situé avant "mille".
            // - il n'est pas suivi de dizaines ni de chiffres (remainder == 0).
            // - il strictement supérieur à 100 (hundreds > 1).
            if (numeralAdjective == NumeralAdjective.Cardinal &&
                hundreds > 1 &&
                thousandPower != 1 &&
                thousandPower != 5 &&
                tensAndUnits == 0)
                return "s";
            else
                return string.Empty;
        }

        /// <summary>
        /// Accorder la puissance de mille donnée.
        /// </summary>
        /// <returns>Un "s" si le nombre strictement supérieur à 1 et n'est pas "mille".
        /// Chaîne vide sinon.</returns>
        private static string MakeThousandPowerAgree(long value, int thousandPower)
        {
            // La puissance prend un "s" à la fin, si et seulement si :
            // - il strictement supérieur à 1.
            // - il n'est pas "mille".
            // - ce n'est pas les centaines et unités.
            if (value > 1 &&
                thousandPower != 1 &&
                thousandPower != 5 &&
                thousandPower != 6)
                return "s";
            else
                return string.Empty;
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="negative">Le nombre est négatif.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        private static string InnerSpell(ulong value, bool negative, Gender gender, NumeralAdjective numeralAdjective)
        {
            // Zéro :
            if (value == 0)
                return "zéro";

            StringBuilder result = new StringBuilder();

            // Valeurs de 1 à 99
            // (court-circuite la méthode HighNumbersSpell() pour de meilleures performances) :
            if (value < 100)
                result.AppendFormat("{0}{1}",
                    NumbersIntoWordsFr._UNITSANDTENS[value],
                    NumbersIntoWordsFr.MakeTwentyAgree((int)value, (NumbersIntoWordsFr._THOUSANDPOWERS.Length - 1), numeralAdjective));

            // Valeurs de 100 à 999
            // (court-circuite la méthode HighNumbersSpell() pour de meilleures performances) :
            else if (value < 1000)
                result.Append(NumbersIntoWordsFr.HundredsAndUnitsSpell((int)value,
                    (NumbersIntoWordsFr._THOUSANDPOWERS.Length - 1),
                    numeralAdjective));
            else
                result.Append(NumbersIntoWordsFr.HighNumbersSpell(value, numeralAdjective));

            // Négatif :
            if (negative)
                result.Insert(0, NumbersIntoWordsFr._MINUS + " ");

            // Genre :
            if (gender == Gender.Feminine && result[result.Length - 2] == 'u' && result[result.Length - 1] == 'n')
                result.Append("e");

            return result.ToString().TrimEnd(' ');
        }

        /// <summary>
        /// Convertit le nombre entier en toutes lettres (nombres supérieurs à 1000).
        /// </summary>
        /// <param name="value">Nombre entier (supérieur à 1000).</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        private static string HighNumbersSpell(ulong value, NumeralAdjective numeralAdjective)
        {
            StringBuilder result = new StringBuilder();
            ulong remainder = 0;
            ulong quotient = value;
            int[] groups = new int[7] { 0, 0, 0, 0, 0, 0, 0 };
            int groupIndex = groups.Length - 1;

            // Découper le nombre en groupes de trois chiffres de la façon suivante :
            // [0]trillions,[1]milliers de billions,[2]billions,[3]milliards,
            // [4]millions,[5]milliers,[6]centaines et unités.
            while (quotient >= 1)
            {
                remainder = quotient % 1000;
                quotient = quotient / 1000; //Math.DivRem(quotient, 1000, out remainder);
                groups[groupIndex] = (int)remainder;
                groupIndex--;
            }

            // Générer le nombre en toutes lettres :
            for (groupIndex = 0; groupIndex < groups.Length; groupIndex++)
            {
                if (groups[groupIndex] == 0)
                    continue;

                if (groupIndex == 1)
                {
                    // Nombre de milliers (> 1 car "un mille" n'existe pas) :
                    if (groups[1] > 1)
                        result.AppendFormat("{0} ", NumbersIntoWordsFr.HundredsAndUnitsSpell(groups[groupIndex], groupIndex, numeralAdjective));
                    // Puissance des milliers (de billions) :
                    result.AppendFormat("{0} ", NumbersIntoWordsFr._THOUSANDPOWERS[groupIndex]);
                    // Ajouter la "billions" dans cette boucle, si le groupe des billions est nul :
                    if (groups[2] == 0)
                        // Dans ce cas, billions prend un "s" car il y en a plus de mille.
                        result.AppendFormat("{0}s ", NumbersIntoWordsFr._THOUSANDPOWERS[2]);
                }
                // Exception "un mille" donne "mille" :
                else if (groupIndex == 5 && groups[5] == 1)
                    result.AppendFormat("{0} ", NumbersIntoWordsFr._THOUSANDPOWERS[groupIndex]);
                // Cas général :
                else
                    result.AppendFormat("{0} {1}{2} ",
                        NumbersIntoWordsFr.HundredsAndUnitsSpell(groups[groupIndex], groupIndex, numeralAdjective),
                        NumbersIntoWordsFr._THOUSANDPOWERS[groupIndex],
                        NumbersIntoWordsFr.MakeThousandPowerAgree(groups[groupIndex], groupIndex));
            }

            return result.ToString().TrimEnd(' ');
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres (nombres entre 0 et 999).
        /// </summary>
        /// <param name="value">Nombre entier (entre 0 et 999).</param>
        /// <param name="thousandPower">La puissance de mille du nombre.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        private static string HundredsAndUnitsSpell(int value, int thousandPower, NumeralAdjective numeralAdjective)
        {
            if (value == 0)
                return string.Empty;

            int remainder = 0;
            int quotient = Math.DivRem(value, 100, out remainder);

            if (quotient > 0)
            {
                if (remainder > 0)
                    return string.Format("{0}{1} {2}{3}",
                        NumbersIntoWordsFr._HUNDREDS[quotient],
                        NumbersIntoWordsFr.MakeHundredAgree(quotient, remainder, thousandPower, numeralAdjective),
                        NumbersIntoWordsFr._UNITSANDTENS[remainder],
                        NumbersIntoWordsFr.MakeTwentyAgree(remainder, thousandPower, numeralAdjective));
                else
                    return string.Format("{0}{1}",
                        NumbersIntoWordsFr._HUNDREDS[quotient],
                        NumbersIntoWordsFr.MakeHundredAgree(quotient, remainder, thousandPower, numeralAdjective));
            }
            else
                return string.Format("{0}{1}",
                    NumbersIntoWordsFr._UNITSANDTENS[remainder],
                    NumbersIntoWordsFr.MakeTwentyAgree(remainder, thousandPower, numeralAdjective));
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(int value)
        {
            return NumbersIntoWordsFr.Spell(value, Gender.Masculine);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(int value, Gender gender)
        {
            return NumbersIntoWordsFr.Spell(value, gender, NumeralAdjective.Cardinal);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(int value, Gender gender, NumeralAdjective numeralAdjective)
        {
            if (value == int.MinValue)
                return NumbersIntoWordsFr.InnerSpell((ulong)Math.Abs((long)value), true, gender, numeralAdjective);

            return NumbersIntoWordsFr.InnerSpell((ulong)Math.Abs(value), (value < 0), gender, numeralAdjective);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(uint value)
        {
            return NumbersIntoWordsFr.Spell(value, Gender.Masculine);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(uint value, Gender gender)
        {
            return NumbersIntoWordsFr.Spell(value, gender, NumeralAdjective.Cardinal);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(uint value, Gender gender, NumeralAdjective numeralAdjective)
        {
            return NumbersIntoWordsFr.InnerSpell((ulong)value, false, gender, numeralAdjective);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(long value)
        {
            return NumbersIntoWordsFr.Spell(value, Gender.Masculine);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(long value, Gender gender)
        {
            return NumbersIntoWordsFr.Spell(value, gender, NumeralAdjective.Cardinal);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(long value, Gender gender, NumeralAdjective numeralAdjective)
        {
            if (value == long.MinValue)
                return NumbersIntoWordsFr.InnerSpell((ulong)(-value), true, gender, numeralAdjective);

            return NumbersIntoWordsFr.InnerSpell((ulong)Math.Abs(value), (value < 0), gender, numeralAdjective);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(ulong value)
        {
            return NumbersIntoWordsFr.Spell(value, Gender.Masculine);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(ulong value, Gender gender)
        {
            return NumbersIntoWordsFr.Spell(value, gender, NumeralAdjective.Cardinal);
        }

        /// <summary>
        /// Convertit un nombre entier en toutes lettres.
        /// </summary>
        /// <param name="value">Nombre entier.</param>
        /// <param name="gender">Genre du nombre entier.</param>
        /// <param name="numeralAdjective">Nature de l'adjectif numéral.</param>
        /// <returns>Le nombre en toutes lettres.</returns>
        public static string Spell(ulong value, Gender gender, NumeralAdjective numeralAdjective)
        {
            return NumbersIntoWordsFr.InnerSpell(value, false, gender, numeralAdjective);
        }

        #endregion
    }
}