using System;

class RationalNumber
{
    private int numerator;     // чисельник
    private int denominator;   // знаменник

    public int Numerator
    {
        get { return numerator; }
        set { numerator = value; }
    }
    public int Denominator
    {
        get { return denominator; }
        set
        {
            if (value == 0) throw new ArgumentException("Знаменник немає значити нулю");
            denominator = value;
        }
    }
    public RationalNumber(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }
    // Збільшує дріб на 1
    public void Increment()
    {
        numerator += denominator;
    }
    public override string ToString()
    {
        return $"{numerator}/{denominator}";
    }
}
class FractionArray
{
    private RationalNumber[] fractions; // масив дробів

    public FractionArray(int size)
    {
        fractions = new RationalNumber[size];
    }
    // індексатор
    public RationalNumber this[int index]
    {
        get { return fractions[index]; }
        set { fractions[index] = value; }
    }
    // оператор ++ (збільшує всі дроби на 1)
    public static FractionArray operator ++(FractionArray arr)
    {
        for (int i = 0; i < arr.fractions.Length; i++)
        {
            arr.fractions[i]?.Increment();
        }
        return arr;
    }
    public override string ToString()
    {
        string result = "";
        for (int i = 0; i < fractions.Length; i++)
        {
            result += fractions[i]?.ToString() + " ";
        }
        return result.Trim();
    }
}
class Program
{
    static void Main(string[] args)
    {
        FractionArray arr = new FractionArray(3);
        arr[0] = new RationalNumber(1, 2);  // 1/2
        arr[1] = new RationalNumber(2, 3);  // 2/3
        arr[2] = new RationalNumber(3, 4);  // 3/4

        Console.WriteLine("Масив дробів:");
        Console.WriteLine(arr);
        arr++; // оператор ++
        Console.WriteLine("\nПісля ++ :");
        Console.WriteLine(arr);
    }
}
