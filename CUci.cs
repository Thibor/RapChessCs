using System;

namespace RapCsChess
{
    class CUci
    {
        public string command;
        public string[] tokens;

        public int GetIndex(string key,int def)
        {
            for(int n = 0;n < tokens.Length;n++)
            {
                if (tokens[n] == key)
                {
                    return n + 1;
                }
            }
            return def;
        }

        public int GetInt(string key,int def)
        {
            for (int n = 0; n < tokens.Length - 1; n++)
            {
                if (tokens[n] == key)
                {
                    return Int32.Parse(tokens[n +1]);
                }
            }
            return def;
        }

        public void SetMsg(string msg)
        {
            tokens = msg.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            command = tokens[0];
        }
    }
}
