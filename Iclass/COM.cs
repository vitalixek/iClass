using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Iclass
{
    public class COM
    {
        private Beta_COM.ICOM_Cls k = new Beta_COM.COM_Cls();
        public int DictionaryElements() // Кол-во элемнтов в словаре
        {
            int dictionary_elements = k.Dictionary_all_elements;
            return dictionary_elements;
        }
        
        public string GetNumber(int dictionary_number) //получить элемент словаря
        {
            string number = k.GetElementByNumber[dictionary_number];
            return number;
        }


        public int DictionaryAttributes() // Кол-во признаков
        {
            int dictionary_attributes = k.Dictionary_parameters;
            return dictionary_attributes;
        }


        public int DictionaryClasses() // Кол-во классов
        {
            int dictionary_classes = k.Dictionary_classes;
            return dictionary_classes;
        }


        public void Open(string path) //Открытие файла типа fsc 
        {
            k.Init = path;
           
        }

        public void Send(DataTable dt)
        {
           
                //k.ValueAsString() //ЗАБИВКА ЗНАЧЕНИЙ ОБЪЕКТА
            foreach (DataRow dr in dt.Rows)
            {
                k.ValueAsString[1, dr[0].ToString()] = dr[1].ToString();
            }
        }
        //Выполнить Execute
        public DataTable Receive(DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                dr[1] = Convert.ToString(k.ValueAsString[1, dr[0].ToString()]);
            }
            return dt;
        }

        public void NumberOfObjects(int value) // Коп-во объектов
        {
            k.N_ob = value;
        }

        public void Store() // Сохранение результатов классификации
        {
            k.StoreData = @"D:\project\FuzzyClassifier\iiii.fsc";
            MessageBox.Show("Данные классификации успешно сохранены.");
        }

        public void Destroy(int number_of_classifier) // уничтожение классификатора
        {
            k.Destroy = number_of_classifier;
        } 

        public void Execute()
        {
            k.Execute();
        }

    }
}
    

