using TPUM.XmlShared.Generated;

namespace TPUM.Server.Presentation.Factory
{
    internal static class XmlResponseFactory
    {
        public static Response CreateResponse(ResponseType type, ResponseContent content)
        {
            return new Response
            {
                ContentType = type,
                Item = content
            };
        }
    }
}