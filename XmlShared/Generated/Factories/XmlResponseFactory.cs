namespace TPUM.XmlShared.Generated.Factory
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