using AutoFixture;

namespace Messenger.Identity.ComponentTests;

public static class TestData
{
    private static readonly Fixture Fixture = new ();

    public static class User
    {
        public static readonly string Email = Fixture.Create<string>() + "@test.com";
        public static readonly string Password = Fixture.Create<string>();
        public static readonly string WrongPassword = Fixture.Create<string>() + Password;
    }

    public static class RefreshToken
    {
        public static readonly string Valid = Fixture.Create<string>();
        public static readonly string Used = Fixture.Create<string>();
        public static readonly string New = Fixture.Create<string>();
    }
}
