﻿using System.Collections.Generic;
using System.Linq;
using Gauge.CSharp.Lib;
using Gauge.Messages;
using Google.ProtocolBuffers;

namespace Gauge.CSharp.Runner.Communication
{
    public class GaugeApiConnection : AbstractGaugeConnection
    {
        public GaugeApiConnection(ITcpClientWrapper clientWrapper) : base(clientWrapper)
        {
        }

        public IEnumerable<string> GetStepValue(IEnumerable<string> stepTexts, bool hasInlineTable)
        {
            foreach (var stepText in stepTexts)
            {
                var stepValueRequest = GetStepValueRequest.CreateBuilder()
                    .SetStepText(stepText)
                    .SetHasInlineTable(hasInlineTable)
                    .Build();
                var stepValueRequestMessage = APIMessage.CreateBuilder()
                    .SetMessageId(GenerateMessageId())
                    .SetMessageType(APIMessage.Types.APIMessageType.GetStepValueRequest)
                    .SetStepValueRequest(stepValueRequest)
                    .Build();
                var apiMessage = WriteAndReadApiMessage(stepValueRequestMessage);
                yield return apiMessage.StepValueResponse.StepValue.StepValue;
            }
        }

        public APIMessage WriteAndReadApiMessage(IMessageLite stepValueRequestMessage)
        {
            lock (TcpClientWrapper)
            {
                WriteMessage(stepValueRequestMessage);
                return ReadMessage();
            }
        }

        private APIMessage ReadMessage()
        {
            var responseBytes = ReadBytes();
            return APIMessage.ParseFrom(responseBytes.ToArray());
        }
    }
}