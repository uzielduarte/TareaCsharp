﻿using Core.Poco;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Data
{
    public class RAFContext
    {
        private string fileName;
        private int size;

        public RAFContext(string fileName, int size)
        {
            this.fileName = fileName;
            this.size = size;
        }

        public Stream HeaderStream
        {
            get => File.Open($"{fileName}.hd", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public Stream DataStream
        {
            get => File.Open($"{fileName}.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }

        public void Create<T>(T t)
        {
            using (BinaryWriter bwHeader = new BinaryWriter(HeaderStream),
                                  bwData = new BinaryWriter(DataStream))
            {
                int n, k;
                using (BinaryReader brHeader = new BinaryReader(bwHeader.BaseStream))
                {
                    if (brHeader.BaseStream.Length == 0)
                    {
                        n = 0;
                        k = 0;
                    }
                    else
                    {
                        brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                        n = brHeader.ReadInt32();
                        k = brHeader.ReadInt32();
                    }

                    long pos = k * size;
                    bwData.BaseStream.Seek(pos, SeekOrigin.Begin);

                    PropertyInfo[] info = t.GetType().GetProperties();
                    foreach (PropertyInfo pinfo in info)
                    {
                        Type type = pinfo.PropertyType;
                        object obj = pinfo.GetValue(t, null);

                        if (type.IsGenericType)
                        {
                            continue;
                        }

                        if (pinfo.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                        {
                            bwData.Write(++k);
                            continue;
                        }

                        if (type == typeof(int))
                        {
                            bwData.Write((int)obj);
                        }
                        else if (type == typeof(long))
                        {
                            bwData.Write((long)obj);
                        }
                        else if (type == typeof(float))
                        {
                            bwData.Write((float)obj);
                        }
                        else if (type == typeof(double))
                        {
                            bwData.Write((double)obj);
                        }
                        else if (type == typeof(decimal))
                        {
                            bwData.Write((decimal)obj);
                        }
                        else if (type == typeof(char))
                        {
                            bwData.Write((char)obj);
                        }
                        else if (type == typeof(bool))
                        {
                            bwData.Write((bool)obj);
                        }
                        else if (type == typeof(string))
                        {
                            bwData.Write((string)obj);
                        }
                    }

                    long posH = 8 + n * 4;
                    bwHeader.BaseStream.Seek(posH, SeekOrigin.Begin);
                    bwHeader.Write(k);

                    bwHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                    bwHeader.Write(++n);
                    bwHeader.Write(k);
                }
            }
        }


        #region BinarySearch implemtation

        private static int BinarySearch(int[] array, int item)
        {
            int left = 0;
            int right = array.Length - 1;

            while (left <= right)
            {
                var middle = (left + right) / 2;

                if (array[middle] == item)
                {
                    return middle;
                }

                if (item < array[middle])
                {
                    right = middle - 1;
                }
                else
                {
                    left = middle + 1;
                }
            }
            return -1;
        }

        #endregion

        public T Get<T>(int id)
        {
            T newValue = (T)Activator.CreateInstance(typeof(T));
            using (BinaryReader brHeader = new BinaryReader(HeaderStream),
                                brData = new BinaryReader(DataStream))
            {
                brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                int n = brHeader.ReadInt32();
                int k = brHeader.ReadInt32();

                if (id <= 0 || id > k)
                {
                    return default(T);
                }

                PropertyInfo[] properties = newValue.GetType().GetProperties();
                long posh = 8 + (id - 1) * 4;
                brHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                int index = id; //brHeader.ReadInt32();
                //Console.WriteLine(posh + " " + index);
                long posd = (index - 1) * size;
                brData.BaseStream.Seek(posd, SeekOrigin.Begin);
                foreach (PropertyInfo pinfo in properties)
                {
                    Type type = pinfo.PropertyType;

                    if (type.IsGenericType)
                    {
                        continue;
                    }

                    if (type == typeof(int))
                    {
                        pinfo.SetValue(newValue, brData.GetValue<int>(TypeCode.Int32));
                    }
                    else if (type == typeof(long))
                    {
                        pinfo.SetValue(newValue, brData.GetValue<long>(TypeCode.Int64));
                    }
                    else if (type == typeof(float))
                    {
                        pinfo.SetValue(newValue, brData.GetValue<float>(TypeCode.Single));
                    }
                    else if (type == typeof(double))
                    {
                        pinfo.SetValue(newValue, brData.GetValue<double>(TypeCode.Double));
                    }
                    else if (type == typeof(decimal))
                    {
                        pinfo.SetValue(newValue, brData.GetValue<decimal>(TypeCode.Decimal));
                    }
                    else if (type == typeof(char))
                    {
                        pinfo.SetValue(newValue, brData.GetValue<char>(TypeCode.Char));
                    }
                    else if (type == typeof(bool))
                    {
                        pinfo.SetValue(newValue, brData.GetValue<bool>(TypeCode.Boolean));
                    }
                    else if (type == typeof(string))
                    {
                        pinfo.SetValue(newValue, brData.GetValue<string>(TypeCode.String));
                    }
                }

                return newValue;
            }

        }

        public List<T> GetAll<T>()
        {
            List<T> listT = new List<T>();
            int n, k;
            using (BinaryReader brHeader = new BinaryReader(HeaderStream))
            {
                if (brHeader.BaseStream.Length == 0)
                    return null;
                brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                n = brHeader.ReadInt32();
                k = brHeader.ReadInt32();
            }

            for (int i = 0; i < n; i++)
            {
                int index;
                using (BinaryReader brHeader = new BinaryReader(HeaderStream))
                {
                    long posh = 8 + i * 4;
                    brHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                    index = brHeader.ReadInt32();
                }
                
                T t = Get<T>(index);
                
                listT.Add(t);
            }

            return listT;
        }

        public int Update<T>(T t)
        {
            // El primer paso es conseguir el id del generico T que actualizaremos
            PropertyInfo[] info = t.GetType().GetProperties();
            int id = -1;
            
            foreach(PropertyInfo pinfo in info)
            {
                if (pinfo.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                {
                    id = int.Parse(pinfo.GetValue(t).ToString());
                    //Console.WriteLine(pinfo.Name + ": " + id);
                }
            }

            if (id == -1)
                return -1;

            using ( BinaryWriter bwData = new BinaryWriter(DataStream))
            {
                using (BinaryReader brHeader = new BinaryReader(HeaderStream))
                {
                    brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                    int n = brHeader.ReadInt32();
                    int k = brHeader.ReadInt32();

                    if (id <= 0 || id > k)
                    {
                        return -1;
                    }

                    long posh = 8 + (id - 1) * 4;
                    brHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                    int index = brHeader.ReadInt32();

                    long posd = (index - 1) * size;
                    bwData.BaseStream.Seek(posd, SeekOrigin.Begin);

                    PropertyInfo[] properties = t.GetType().GetProperties();
                    foreach (PropertyInfo pinfo in properties)
                    {
                        Type type = pinfo.PropertyType;
                        object obj = pinfo.GetValue(t, null);

                        if (type.IsGenericType)
                        {
                            continue;
                        }

                        if (pinfo.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                        {
                            bwData.Write(id);
                            continue;
                        }

                        if (type == typeof(int))
                        {
                            bwData.Write((int)obj);
                        }
                        else if (type == typeof(long))
                        {
                            bwData.Write((long)obj);
                        }
                        else if (type == typeof(float))
                        {
                            bwData.Write((float)obj);
                        }
                        else if (type == typeof(double))
                        {
                            bwData.Write((double)obj);
                        }
                        else if (type == typeof(decimal))
                        {
                            bwData.Write((decimal)obj);
                        }
                        else if (type == typeof(char))
                        {
                            bwData.Write((char)obj);
                        }
                        else if (type == typeof(bool))
                        {
                            bwData.Write((bool)obj);
                        }
                        else if (type == typeof(string))
                        {
                            bwData.Write((string)obj);
                        }
                    }
                    return 0;
                }
            }
        }

        public bool Delete<T>(T t)
        {
            PropertyInfo[] info = t.GetType().GetProperties();
            int id = -1;

            foreach (PropertyInfo pinfo in info)
            {
                if (pinfo.Name.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                {
                    id = int.Parse(pinfo.GetValue(t).ToString());
                    break;
                }
            }

            if (id == -1)
                return false;
            
            using (BinaryWriter tempBWHeader = new BinaryWriter(File.Open($"temp.hd", FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                int n;
                int k;
                int tempN = 0;
                //int tempK = 0;
                using (BinaryReader brHeader = new BinaryReader(HeaderStream))
                {
                    if (brHeader.BaseStream.Length == 0)
                        return false;

                    brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                    n = brHeader.ReadInt32();
                    k = brHeader.ReadInt32();
                }
                if(n == 1)
                {
                    File.Delete(fileName);
                    return false;
                }
                //HeaderStream.Close();
                int tempIterator = 0;
                for (int i = 0; i < n; i++)
                {
                    int index;
                    
                    using (BinaryReader brHeader = new BinaryReader(HeaderStream))
                    {
                        long posh = 8 + i * 4;
                        brHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                        index = brHeader.ReadInt32();

                        if (index == id)
                            continue;
                        //tempK++;
                        long posH = 8 + tempIterator * 4;
                        tempBWHeader.BaseStream.Seek(posH, SeekOrigin.Begin);
                        tempBWHeader.Write(index);

                        tempBWHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                        tempBWHeader.Write(++tempN);
                        tempBWHeader.Write(index);
                        //Console.WriteLine(i + " => " + tempIterator + " => " + index);
                        tempIterator++;
                    }
                }
                File.Delete(fileName);

            }

            using (BinaryWriter bwHeader = new BinaryWriter(HeaderStream))
            {
                int n;
                int k;
                int tempN = 0;
                
                using (BinaryReader tempBRHeader = new BinaryReader(File.Open($"temp.hd", FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                {
                    tempBRHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                    n = tempBRHeader.ReadInt32();
                    k = tempBRHeader.ReadInt32();
                    for (int i = 0; i < n; i++)
                    {
                        int index;
                        long posh = 8 + i * 4;
                        tempBRHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                        index = tempBRHeader.ReadInt32();

                        long posH = 8 + i * 4;
                        bwHeader.BaseStream.Seek(posH, SeekOrigin.Begin);
                        bwHeader.Write(index);

                        bwHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                        bwHeader.Write(++tempN);
                        bwHeader.Write(index);
                        //Console.WriteLine(posH + " => " + index + " " + tempN);
                    }
                }
                    
            }

            List<Product> listT = new List<Product>();
            int nn, kk;
            using (BinaryReader brHeader = new BinaryReader(File.Open($"temp.hd", FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                if (brHeader.BaseStream.Length == 0)
                    return false;
                brHeader.BaseStream.Seek(0, SeekOrigin.Begin);
                nn = brHeader.ReadInt32();
                kk = brHeader.ReadInt32();
                
            }

            for (int i = 0; i < nn; i++)
            {
                int index;
                using (BinaryReader brHeader = new BinaryReader(File.Open($"temp.hd", FileMode.OpenOrCreate, FileAccess.ReadWrite)))
                {
                    long posh = 8 + i * 4;
                    brHeader.BaseStream.Seek(posh, SeekOrigin.Begin);
                    index = brHeader.ReadInt32();
                    //Console.WriteLine(index);
                }

                Product p = Get<Product>(index);
                listT.Add(p);
            }
            /*foreach (Product pr in listT)
            {
                Console.WriteLine($"Id: {pr.Id} Name: {pr.Name}\n");
            }*/
            
            if (File.Exists("temp.hd"))
                File.Delete("temp.hd");
            return true;
        }
    }
}
