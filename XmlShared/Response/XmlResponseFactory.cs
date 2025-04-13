namespace TPUM.XmlShared.Response
{
    internal static class XmlResponseFactory
    {
        public static Response CreateResponse(ResponseType type, ResponseContent content)
        {
            return new Response
            {
                ContentType = type,
                Content = content
            };
        }
    }
}
