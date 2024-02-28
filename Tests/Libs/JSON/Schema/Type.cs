using JBool = JSON.Schema.Type.Boolean;
using JFloat = JSON.Schema.Type.Number<float>;
using JInt = JSON.Schema.Type.Number<int>;
using JString = JSON.Schema.Type.String;
using JInvalidData = JSON.Schema.InvalidDataException;

namespace KibbleCabal.Tests.Libs.JSON.Schema.Type;

public class Boolean
{
    [Test]
    public void IsValid()
    {
        Assert.IsTrue(new JBool().IsValid(true));
        Assert.IsTrue(new JBool().IsValid(false));
    }

    [Test]
    public void NumberIsNotValid() => Assert.That(new JBool().IsValid(1), Is.False);

    [Test]
    public void Clean() => Assert.That(new JBool().Clean(false).VariantEquals(false), Is.True);

    [Test]
    public void FailToCleanNumber() => Assert.Catch(typeof(JInvalidData), () => new JBool().Clean(1));
}

public class Float
{
    [Test]
    public void IsValid() => Assert.That(new JFloat().IsValid(1.0f), Is.True);

    [Test]
    public void IsNotValid() => Assert.That(new JFloat().IsValid(false), Is.False);

    [Test]
    public void Clean() => Assert.That(new JFloat().Clean(1.5f).VariantEquals(1.5f), Is.True);

    [Test]
    public void FailToClean() => Assert.Catch(typeof(JInvalidData), () => new JFloat().Clean(false));
}

public class Int
{
    [Test]
    public void IsValid() => Assert.IsTrue(new JInt().IsValid(0));
    
    [Test]
    public void IsNotValid() => Assert.IsFalse(new JInt().IsValid(new Godot.Collections.Array()));

    [Test]
    public void Clean() => Assert.That(new JInt().Clean(2).VariantEquals(2), Is.True);

    [Test]
    public void FailToClean() => Assert.Catch(typeof(JInvalidData), () => new JInt().Clean(2.5f));
}

public class String
{
    private const string TestString = "some string";
    
    [Test]
    public void IsValid() => Assert.That(new JString().IsValid(TestString), Is.True);

    [Test]
    public void IsNotValid() => Assert.That(new JString().IsValid(Colors.Black), Is.False);

    [Test]
    public void Clean() => Assert.That(new JString().Clean(TestString).VariantEquals(TestString), Is.True);

    [Test]
    public void FailToClean() => Assert.Catch(typeof(JInvalidData), () => new JString().Clean(100));
}