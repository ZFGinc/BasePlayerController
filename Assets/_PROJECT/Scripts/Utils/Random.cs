namespace ZFGinc.Utils
{
    public static class Random
    {
        public static int Seed { get; private set; }

        private static System.Random random;
        private static bool Initialized = false;
        private const int m = 1000000;

        public static void Init(int seed = -1)
        {
            if (Seed < 0) Seed = random.Next(0, 99999999);
            else random = new System.Random(seed);

            Initialized = true;
        }

        public static int GetRange(int start, int end)
        {
            if (!Initialized) throw new System.Exception("Need initialization!");

            return random.Next(start, end);
        }

        public static float GetRange(int start, float end)
        {
            if (!Initialized) throw new System.Exception("Need initialization!");

            int s = start * m;
            int e = (int)(end * m);

            int res = random.Next(s, e);

            return float.Parse(res.ToString()) / m;
        }

        public static float GetRange(float start, int end)
        {
            if (!Initialized) throw new System.Exception("Need initialization!");

            int s = (int)(start * m);
            int e = end * m;

            int res = random.Next(s, e);

            return float.Parse(res.ToString()) / m;
        }

        public static float GetRange(float start, float end)
        {
            if (!Initialized) throw new System.Exception("Need initialization!");

            int s = (int)(start * m);
            int e = (int)(end * m);

            int res = random.Next(s, e);

            return float.Parse(res.ToString()) / m;
        }
    }
}