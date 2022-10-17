using AlphaX.Parserz;
using AlphaX.Parserz.Interfaces;
using AlphaX.Parserz.Results;
using System.Text;

namespace AlphaX.FormulaEngine
{
    internal class StringValueParser : Parser<StringResult>
    {
        protected override IParserState ParseInput(IParserState inputState)
        {
            if (!inputState.Input.StartsWith("\""))
                return CreateErrorState(inputState, new ParserError(inputState.Index, "Unexpected input. Expected '\"'."));

            var input = inputState.Input;
            var strBuffer = new StringBuilder();
            for (int index = 1; index < input.Length; index++)
            {
                if (input[index] == '"' && (index != input.Length - 1 && input[index + 1] == '"'))
                {
                    strBuffer.Append(input[index]);
                    index++;
                }
                else if (input[index] == '"' && (index == input.Length - 1 || input[index + 1] != '"'))
                {
                    return CreateResultState(inputState, new StringResult(strBuffer.ToString()), inputState.Index + index + 1);
                }
                else
                {
                    strBuffer.Append(input[index]);
                }
            }

            return CreateErrorState(inputState, new ParserError(inputState.Index, "Unexpected input. Expected a string value"));
        }
    }
}
