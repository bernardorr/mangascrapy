using System;

namespace MangaScrapping
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            Console.WriteLine("Ingresa la URL: ");
            string url = Console.ReadLine();
            Console.WriteLine("¿Cuantos segundos esperar para obtener la siguiente página?: ");
            int seconds = int.Parse(Console.ReadLine());
            MangaScraping mangaScraping = new MangaScraping(url,seconds,(currentPage,totalPages)=> { Console.WriteLine($"Se obtuvo la imagen {currentPage} de {totalPages}"); });
            Console.WriteLine($"Se descargaran {mangaScraping.Pages} de {mangaScraping.Title}");
            Console.WriteLine($"Revisa el archivo {mangaScraping.GetPdf()}");
            Console.ReadLine();

            

        }
    }
}
