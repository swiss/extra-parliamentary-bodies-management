using System.Text;

namespace Bk.APG.Common.Utilities;

public static class SimilaritySearchTransformer
{
    public static string Reduce(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var transformed = value.Trim().ToLower();
        transformed = ReduceSpecialCharacters(transformed);
        transformed = ReduceDifferentTypesOfI(transformed);
        transformed = ReduceDoubleCharacters(transformed);

        return transformed;
    }

    private static string ReduceDoubleCharacters(string aValue)
    {
        if (aValue.Length < 2)
        {
            return aValue;
        }

        var value = aValue.ToCharArray();
        var result = new StringBuilder();

        result.Append(value[0]);

        for (var i = 1; i < value.Length; i++)
        {
            if (value[i] != value[i - 1])
            {
                result.Append(value[i]);
            }
        }

        return result.ToString();
    }

    private static string ReduceDifferentTypesOfI(string aValue)
    {
        var transformed = aValue.Replace('y', 'i');
        transformed = transformed.Replace('j', 'i');

        return transformed;
    }

    private static string ReduceSpecialCharacters(string aValue)
    {
        var transformed = aValue;

        var specialToReducedCharacterMap = new Dictionary<string, string>
            {
                { "ue", "u" },
                { "oe", "o" },
                { "ae", "a" },

                { "ä", "a" },
                { "á", "a" },
                { "à", "a" },
                { "â", "a" },
                { "ã", "a" },
                { "å", "a" },
                { "æ", "a" },
                { "ā", "a" },
                { "ą", "a" },
                { "ă", "a" },

                { "ç", "c" },
                { "č", "c" },
                { "ċ", "c" },
                { "ć", "c" },

                { "ð", "d" },
                { "đ", "d" },
                { "ď", "d" },

                { "é", "e" },
                { "è", "e" },
                { "ê", "e" },
                { "ë", "e" },
                { "ē", "e" },
                { "ė", "e" },
                { "ę", "e" },
                { "ě", "e" },

                { "ģ", "g" },
                { "ġ", "g" },
                { "ğ", "g" },

                { "ħ", "h" },

                { "í", "i" },
                { "ì", "i" },
                { "î", "i" },
                { "ï", "i" },
                { "ĩ", "i" },
                { "ī", "i" },
                { "į", "i" },
                { "ı", "i" },

                { "ķ", "k" },

                { "ŀ", "l" },
                { "ļ", "l" },
                { "ł", "l" },
                { "ľ", "l" },
                { "ĺ", "l" },

                { "ñ", "n" },
                { "ņ", "n" },
                { "ń", "n" },
                { "ň", "n" },

                { "ö", "o" },
                { "ó", "o" },
                { "ò", "o" },
                { "ô", "o" },
                { "õ", "o" },
                { "ø", "o" },
                { "œ", "o" },
                { "ō", "o" },
                { "ő", "o" },

                { "ŗ", "r" },
                { "ŕ", "r" },
                { "ř", "r" },

                { "ş", "s" },
                { "š", "s" },
                { "ß", "s" },
                { "ś", "s" },

                { "ţ", "t" },
                { "þ", "t" },
                { "ť", "t" },

                { "ü", "u" },
                { "ú", "u" },
                { "ù", "u" },
                { "û", "u" },
                { "ū", "u" },
                { "ų", "u" },
                { "ů", "u" },
                { "ű", "u" },

                { "ý", "y" },
                { "ÿ", "y" },

                { "ž", "z" },
                { "ż", "z" },
                { "ź", "z" },

                { "'", ""},
                { "-", " "}
            };

        foreach (var special in specialToReducedCharacterMap.Keys)
        {
            transformed = transformed.Replace(special, specialToReducedCharacterMap[special]);
        }

        return transformed;
    }
}
