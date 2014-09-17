namespace NetTriple.Annotation.Fluency
{
    public static class BuildTransform
    {
        public static TransformBuilder<T> For<T>(string typeString)
        {
            return new TransformBuilder<T>(typeString);
        }
    }
}
