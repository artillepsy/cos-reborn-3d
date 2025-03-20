using System;
using System.Collections.Generic;

namespace Game.Scripts.Shared.Extensions
{
public static class NumbStrHelper
{
    /// <summary>
    /// Transforms float value into appropriate str format.
    /// </summary>
    /// <param name="value">Affected value.</param>
    /// <param name="isRoundSmallToInt">Rounds values less 1000 to int if true.</param>
    /// <param name="lessOnePrecision">values less than 1 are converted with precision N numbers after comma</param>
    /// <param name="aboveOnePrecision">values more than 1 and less than 1000 are converted with precision N numbers after comma</param>
    public static string ToStr(
        float value,
        bool  isRoundSmallToInt,
        int   lessOnePrecision  = 3,
        int   aboveOnePrecision = 2)
    {
        return ToStr((double)value, isRoundSmallToInt, lessOnePrecision, aboveOnePrecision);
    }
    
    /// <summary>
    /// Transforms int value into appropriate str format.
    /// </summary>
    /// <param name="value">Affected value.</param>
    public static string ToStr(int value)
    {
        return ToStr((double)value, false);
    }
    
    /// <summary>
    /// Transforms double value into appropriate str format.
    /// </summary>
    /// <param name="value">Affected value.</param>
    /// <param name="isRoundSmallToInt">Rounds values less 1000 to int if true.</param>
    /// <param name="lessOnePrecision">values less than 1 are converted with precision N numbers after comma</param>
    /// <param name="aboveOnePrecision">values more than 1 and less than 1000 are converted with precision N numbers after comma</param>
    public static string ToStr(
        double value,
        bool   isRoundSmallToInt,
        int    lessOnePrecision  = 3,
        int    aboveOnePrecision = 2)
    {
        //1 000,		1 000
        //10 000,		10.00k
        //100 000,		100.00k
        //1 000 000,	1.00M
        //10 000 000,	10.00M
        //100 000 000,	100.00M
        
        if (value < 1)
            return isRoundSmallToInt ? ((int)value).ToString() : TruncateDouble(value, lessOnePrecision);
        
        if (value < 1000)
            return isRoundSmallToInt ? ((int)value).ToString() : TruncateDouble(value, aboveOnePrecision);
        
        var rank = 0;
        while (value >= 1000)
        {
            value /= 1000;
            rank  += 1;
        }
        //10 000,		rank = 1,	b = 10.00
        //100 000,		rank = 1,	b = 100.00
        //1 000 000,	rank = 2,	b = 1.00
        //10 000 000,	rank = 2	b = 10.00
        //100 000 000,	rank = 2,	b = 100.00
        
        var intPart  = (int)Math.Floor(value);
        var fracPart = (int)Math.Round((value - intPart) * 100);
        
        if (fracPart == 100)
            fracPart -= 1;
        
        string fracPartStr = intPart switch
        {
            < 100 when fracPart  < 10               => $"0{fracPart.ToString()}",
            >= 100 when fracPart < 10               => 0.ToString(),
            >= 100 when fracPart is >= 10 and < 100 => (fracPart / 10).ToString(),
            _                                       => fracPart.ToString()
        };
        
        return $"{intPart.ToString()}.{fracPartStr}{GetRankChar(rank)}";
    }
    
    private static string TruncateDouble(double number, int precision = 3)
    {
        int    multiplier  = (int)Math.Pow(10, precision);
        double truncNumber = Math.Round(number           * multiplier) / multiplier;
        int    intNumber   = (int)Math.Round(truncNumber * multiplier);
        
        int digitsCount = 0;
        int divisor     = multiplier;
        
        for (int i = precision; i > 0; i--)
        {
            if (intNumber % divisor == 0)
                break;
            
            divisor /= 10;
            digitsCount++;
        }
        return digitsCount == 0 ? ((int)truncNumber).ToString() : truncNumber.ToString($"F{digitsCount}");
    }
    
    private static string GetRankChar(int rank)
    {
        char[] cShort = RanksCharsShort;
        if (rank < cShort.Length)
            return cShort[rank].ToString();
        rank -= cShort.Length;
        
        char[] cLong = RanksCharsLong;
        
        if (rank < cLong.Length)
            return cLong[rank].ToString();
        
        int firstCharIdx = rank / cLong.Length;
        if (firstCharIdx >= cLong.Length)
            throw new Exception($"rank {rank} is too big");
        
        int secondCharIdx = rank % cLong.Length;
        return $"{cLong[firstCharIdx]}{char.ToLower(cLong[secondCharIdx])}";
    }
    
    
    private static readonly char[] RanksCharsShort =
    {
        ' ',
        'k',
        'M',
        'B',
        'T',
        'Q',
        'S',
        'N',
        'U',
        'D'
    };
    
    private static char[] _ranksCharsLong;
    
    private static char[] RanksCharsLong
    {
        get
        {
            if (_ranksCharsLong == null)
            {
                var list = new List<char>();
                for (var c = 'A'; c <= 'Z'; c++)
                    list.Add(c);
                foreach (char c in RanksCharsShort)
                    list.Remove(c);
                _ranksCharsLong = list.ToArray();
            }
            
            return _ranksCharsLong;
        }
    }
}
}