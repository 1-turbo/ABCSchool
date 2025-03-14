namespace Application.Wrappers
{
    public class ResponseWrapper : IResponseWrapper
    {
        public List<string> Messages { get; set; } = [];
        public bool IsSuccessful { get; set; }
        public ResponseWrapper() { }

        // Flag - Failure
        public static IResponseWrapper Fail()
        {
            return new ResponseWrapper()
            {
                IsSuccessful = false
            };
        }

        public static IResponseWrapper Fail(string message)
        {
            return new ResponseWrapper()
            {
                IsSuccessful = false,
                Messages = [message]
            };
        }

        public static IResponseWrapper Fail(List<string> messages)
        {
            return new ResponseWrapper()
            {
                IsSuccessful = false,
                Messages = messages
            };
        }

        public static Task<IResponseWrapper> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static Task<IResponseWrapper> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public static Task<IResponseWrapper> FailAsync(List<string> messages)
        {
            return Task.FromResult(Fail(messages));
        }
        // Flag - Success
    }
}