using AlphaX.Parserz;
using AlphaX.Parserz.Interfaces;

namespace AlphaX.FormulaEngine
{
    public class ResultParser : Parser<IParserResult>
    {
        private IParserResult _result;

        public ResultParser(IParserResult result)
        {
            _result = result;
        }

        protected override IParserState ParseInput(IParserState inputState)
        {
            return CreateResultState(inputState, _result, inputState.Index);
        }
    }
}
