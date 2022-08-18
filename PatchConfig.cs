namespace authpatcher
{
    class PatchConfig
    {
        public long routerLocation;
        public int routerMaxLength;

        public long serverLocation;
        public int serverMaxLength;

        public long serverLengthLocation;

        public PatchConfig(long routerAddr, int routerMax, long serverAddr, int serverMax, long serverLengthAddr)
        {
            routerLocation = routerAddr;
            routerMaxLength = routerMax;
            serverLocation = serverAddr;
            serverMaxLength = serverMax;
            serverLengthLocation = serverLengthAddr;
        }
    }
}
