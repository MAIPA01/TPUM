namespace TPUM.XmlShared.Original.Response.Factory
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