using System;
public class RationalNumber
{
    private int numerator;
    private int denominator;

    public int Numerator
    {
        get { return numerator; }
        set
        {
            numerator = value;
            Simplify();
        }
    }
    public int Denominator
    {
        get { return denominator; }
        set
        {
            if (value == 0)
                throw new ArgumentException("Denominator cannot be zero.");
            denominator = value;
            Simplify();
        }
    }
    public RationalNumber(int numerator, int denominator)
    {
        this.numerator = numerator;
        if (denominator == 0)
            throw new ArgumentException("Denominator cannot be zero.");
        this.denominator = denominator;

        Simplify();
    }

    private void Simplify()
    {
        int gcd = GCD(Math.Abs(numerator), Math.Abs(denominator));
        if (gcd > 1)
        {
            numerator /= gcd;
            denominator /= gcd;
        }
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }
    }
    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a == 0 ? 1 : a;
    }
    public static RationalNumber operator ++(RationalNumber fraction)
    {
        return new RationalNumber(fraction.Numerator + fraction.Denominator, fraction.Denominator);
    }
    public override string ToString()
    {
        return $"{Numerator}/{Denominator}";
    }
}
