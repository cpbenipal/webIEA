using System;

namespace webIEA.Dtos
{
    public static class CommonUtils
    {
        //public static string JSONSerializeFromList<T>(T obj)
        //{
        //    string retVal = String.Empty;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj!.GetType());
        //        serializer.WriteObject(ms, obj);
        //        var byteArray = ms.ToArray();
        //        retVal = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
        //    }
        //    return retVal;
        //}

        //public static string JSONSerialize(object obj)
        //{
        //    string repVal = JsonConvert.SerializeObject(obj);
        //    Console.WriteLine(repVal);
        //    return repVal;
        //}

        public static string GenratePassword()
        {
            string allowedChars = "";

            allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";

            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";

            allowedChars += "1,2,3,4,5,6,7,8,9,0,!,@,#,$,%,&,?";

            char[] sep = { ',' };

            string[] arr = allowedChars.Split(sep);

            string passwordString = "";

            string temp = "";

            Random rand = new Random();

            for (int i = 0; i < 8; i++)

            {

                temp = arr[rand.Next(0, arr.Length)];

                passwordString += temp;

            }

            return passwordString;
        }
    }
}
