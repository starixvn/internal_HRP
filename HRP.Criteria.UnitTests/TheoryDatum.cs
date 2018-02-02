namespace HRP.Criteria.UnitTests
{
    public class TheoryDatum<TSystemUnderTest, TExecptedOutput> : TheoryDatum
    {
        public TSystemUnderTest SystemUnderTest { get; set; }

        public TExecptedOutput ExpectedOutput { get; set; }

        public override object[] ToParameterArray()
        {
            var output = new object[2];
            output[0] = SystemUnderTest;
            output[1] = ExpectedOutput;
            return output;
        }
    }

    public abstract class TheoryDatum
    {
        public abstract object[] ToParameterArray();

        public static TheoryDatum Factory<TSystemUnderTest, TExecptedOutput>(TSystemUnderTest sut, TExecptedOutput expectedOutput)
        {
            return new TheoryDatum<TSystemUnderTest, TExecptedOutput>
            {
                SystemUnderTest = sut,
                ExpectedOutput = expectedOutput
            };
        }
    }
}