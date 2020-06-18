using HtmlAgilityPack;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace MangaScrapping
{
    public class MangaScraping
    {
        private const string MANGA_TITLE_XPATH = "//h1";
        private const string IMAGE_XPATH = "//img[@class='viewer-image viewer-page']";
        private const string PAGES_SELECT_XPATH = "//*[@id='viewer-pages-select']/option[last()-1]";
        private const string SRC = "src";
        private string url;
        private string title;
        private int pages;
        private HtmlDocument doc;
        private int seconds;
        private Action<int,int> callbackImageURLRecovered;

        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(this.title))
                {
                    this.title = doc.DocumentNode.SelectSingleNode(MANGA_TITLE_XPATH).InnerText;
                }
                return this.title;
            }

        }

        public int Pages
        {
            get
            {
                if (this.pages < 0)
                {
                    this.pages = int.Parse(doc.DocumentNode.SelectSingleNode(PAGES_SELECT_XPATH).InnerText);
                }
                return this.pages;

            }
        }
        public MangaScraping(string url, int seconds,Action<int,int> callbackImageURLRecovered)
        {
            HtmlWeb web = new HtmlWeb();
            this.pages = -1;
            this.url = url;
            this.doc = web.Load(url);
            this.seconds = seconds;
            this.callbackImageURLRecovered = callbackImageURLRecovered;


        }

        public IEnumerable<string> GetImageUrls()
        {
            foreach (int page in Enumerable.Range(1, this.Pages))
            {
                string pageUrl = $"{url}/{page}";
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(pageUrl);
                string imgUrl = doc.DocumentNode.SelectSingleNode(IMAGE_XPATH).Attributes[SRC].Value;
                this.callbackImageURLRecovered(page,this.Pages);
                Thread.Sleep(1000 * this.seconds);
                yield return imgUrl;
            }
        }

        public string GetPdf()
        {
            IEnumerable<string> imageUrls = this.GetImageUrls().ToList();
            string fileName = $"{Guid.NewGuid().ToString()}.pdf";

            using (var document = new PdfDocument())
            {
                foreach (var imageUrl in imageUrls)
                {
                    PdfPage page = document.AddPage();
                    WebClient wc = new WebClient();
                    using (MemoryStream stream = new MemoryStream(wc.DownloadData(imageUrl)))
                    {
                        using (XImage img = XImage.FromStream(stream))
                        {

                            page.Width = img.Width;
                            page.Height = img.Height;
                            XGraphics gfx = XGraphics.FromPdfPage(page);
                            gfx.DrawImage(img, 0, 0, img.Width, img.Height);
                        }


                    }
                }
                document.Save(fileName);
                return fileName;

            }
        }
    }
}
