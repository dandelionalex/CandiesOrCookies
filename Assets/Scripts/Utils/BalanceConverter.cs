public static class BalanceConverter 
{
    public static string Convert(int value)
    {
        if (value>= 1000)
        {
            var left = value % 1000;
            return $"{(int)(value /1000)},{left:D3}";
        }
        else
        {
            return value.ToString();
        }
    }
}
