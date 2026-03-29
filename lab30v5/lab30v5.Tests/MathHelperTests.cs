using System;
using Xunit;
using lab30v5;

namespace lab30v5.Tests
{
    public class MathHelperTests
    {
        private readonly MathHelper _math = new MathHelper();

        // 1. Тест IsPrime - успішні сценарії (Theory)
        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(13)]
        [InlineData(97)]
        public void IsPrime_ReturnsTrue_ForPrimeNumbers(int prime)
        {
            Assert.True(_math.IsPrime(prime));
        }

        // 2. Тест IsPrime - неуспішні сценарії та крайні випадки (Theory)
        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(15)]
        public void IsPrime_ReturnsFalse_ForNonPrimeNumbers(int nonPrime)
        {
            Assert.False(_math.IsPrime(nonPrime));
        }

        // 3. Тест Factorial - стандартні значення (Theory)
        [Theory]
        [InlineData(0, 1)] // крайній випадок: 0! = 1
        [InlineData(1, 1)]
        [InlineData(5, 120)]
        [InlineData(10, 3628800)]
        public void Factorial_CalculatesCorrectly_ForValidInputs(int input, long expected)
        {
            Assert.Equal(expected, _math.Factorial(input));
        }

        // 4. Тест Factorial - перевірка викидання виключення (Fact)
        [Fact]
        public void Factorial_ThrowsArgumentException_ForNegativeNumbers()
        {
            Assert.Throws<ArgumentException>(() => _math.Factorial(-1));
        }

        // 5. Тест Fibonacci - перевірка послідовності (Theory)
        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 1)]
        [InlineData(5, 5)]
        [InlineData(6, 8)]
        [InlineData(10, 55)]
        public void Fibonacci_ReturnsCorrectValue_ForValidInputs(int n, long expected)
        {
            Assert.Equal(expected, _math.Fibonacci(n));
        }

        // 6. Тест Fibonacci - перевірка на виключення для від'ємних індексів (Fact)
        [Fact]
        public void Fibonacci_ThrowsArgumentException_ForNegativeNumbers()
        {
            Assert.Throws<ArgumentException>(() => _math.Fibonacci(-5));
        }

        // 7. Тест GCD - стандартні обчислення НСД (Theory)
        [Theory]
        [InlineData(54, 24, 6)]
        [InlineData(48, 18, 6)]
        [InlineData(101, 10, 1)] // взаємно прості числа
        public void GCD_CalculatesCorrectly(int a, int b, int expected)
        {
            Assert.Equal(expected, _math.GCD(a, b));
        }

        // 8. Тест GCD - робота з нулем, де один з параметрів = 0 (Fact)
        [Fact]
        public void GCD_WithZero_ReturnsAbsoluteValueOfOther()
        {
            Assert.Equal(5, _math.GCD(0, 5));
            Assert.Equal(10, _math.GCD(-10, 0));
        }

        // 9. Тест GCD - обидва нулі (Fact)
        [Fact]
        public void GCD_BothZero_ReturnsZero()
        {
            Assert.Equal(0, _math.GCD(0, 0));
        }

        // 10. Тест GCD - обробка від'ємних значень (Theory)
        [Theory]
        [InlineData(-54, -24, 6)]
        [InlineData(-48, 18, 6)]
        [InlineData(12, -8, 4)]
        public void GCD_WithNegativeNumbers_ReturnsPositiveGCD(int a, int b, int expected)
        {
            Assert.Equal(expected, _math.GCD(a, b));
        }
    }
}