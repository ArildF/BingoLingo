using BingoLingo.Server.Sessions;
using FluentAssertions;
using Xunit;

namespace BingoLingo.Server.Tests.Sessions
{
    public class AnswerComparerTest
    {
        private readonly AnswerComparer _comparer = new();

        [InlineData("Un caffè", "Un caffe")]
        [InlineData("Sì", "Si")]
        [InlineData("Una tazza di tè", "Una tazza di te")]
        [InlineData("perché", "perche")]
        [Theory]
        public void EqualExceptForAccents_ShouldCompareAsSame(string s1, string s2)
        {
            _comparer.IsAcceptable(s1, s2).Should().BeTrue();
        }

        [InlineData("Un bicchiere", "un bicchiere")]
        [InlineData("sì", "Si")]
        [InlineData("Una tazza di tè", "una tazza di te")]
        [InlineData("perché", "Perché")]
        [Theory]
        public void EqualExceptForCase_ShouldCompareAsSame(string s1, string s2)
        {
            _comparer.IsAcceptable(s1, s2).Should().BeTrue();
        }

        [InlineData("Un bicchiere!", "Un bicchiere")]
        [InlineData("Sì?", "Si")]
        [InlineData("Una tazza di tè!!!", "Una tazza di te")]
        [InlineData("Perché", "Perché?")]
        [Theory]
        public void EqualExceptForPunctuation_ShouldCompareAsSame(string s1, string s2)
        {
            _comparer.IsAcceptable(s1, s2).Should().BeTrue();
        }

        [InlineData("Un bicchiere d'acqua", "Un bicchiere dacqua")]
        [Theory]
        public void EqualExceptForApostrophe_ShouldNotCompareAsSame(string s1, string s2)
        {
            _comparer.IsAcceptable(s1, s2).Should().BeFalse();
        }
    }
}
