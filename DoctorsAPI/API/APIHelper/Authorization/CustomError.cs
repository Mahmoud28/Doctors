using Seagull.API.APIHelper;

namespace API.Authorization
{
    public class CustomError
    {
        public string Error { get; }

        public CustomError(string message)
        {
            APIJsonResult data = new APIJsonResult();
            data.Access = false;
            data.success = false;
            data.Msg.Add("SessionTimeOut");
            data.data = null;
            Error = message;
        }
    }
}