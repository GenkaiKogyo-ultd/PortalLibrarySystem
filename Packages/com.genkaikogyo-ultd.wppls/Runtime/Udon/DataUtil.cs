using VRC.SDK3.Data;

namespace Wacky612.PortalLibrarySystem2
{
    public static class DataUtil
    {
        public static DataToken Null()
        {
            return new DataToken(TokenType.Null);
        }
        
        public static int ForceInt(DataToken data)
        {
            return data.IsNumber ? (int) data.Number : 0;
        }

        public static string ForceString(DataToken data)
        {
            return (data.TokenType == TokenType.String) ? data.String : "";
        }

        public static bool ForceBool(DataToken data)
        {
            return (data.TokenType == TokenType.Boolean) ? data.Boolean : false;
        }

        public static DataToken ForceValue(DataToken data, string key)
        {
            if (data.TokenType == TokenType.DataDictionary && data.DataDictionary.ContainsKey(key))
            {
                return data.DataDictionary[key];
            }
            else
            {
                return DataUtil.Null();
            }
        }

        public static int ForceValueInt(DataToken data, string key)
        {
            return ForceInt(ForceValue(data, key));
        }

        public static string ForceValueString(DataToken data, string key)
        {
            return ForceString(ForceValue(data, key));
        }

        public static bool ForceValueBool(DataToken data, string key)
        {
            return ForceBool(ForceValue(data, key));
        }

        public static DataList Intersection(DataList a, DataList b)
        {
            var ans = new DataList();

            for (int i = 0; i < a.Count; i++)
            {
                for (int j = 0; j < b.Count; j++)
                {
                    if (a[i].CompareTo(b[j]) == 0) ans.Add(a[i]);
                }
            }

            return ans;
        }
    }
}
