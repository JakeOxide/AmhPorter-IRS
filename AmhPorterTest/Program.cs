using AmhPorterTest.Utils;

namespace AmhPorterTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.InputEncoding = System.Text.Encoding.Unicode;
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

           // SystemManager systemManager = new SystemManager();
           // systemManager.TriggerCorpusPreprocessing();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}















/*
            SystemManager? systemManager = new SystemManager();
            string input = "“የጽሁፍ ማስጠንቀቂያ እየተሰጠው በስድስት ወር ውስጥ ለአምስት ቀናት ከስራ መቅረት የሥራ ውልን ያለ ማስጠንቀቅያ ያቋርጣል፡፡ በሌላ በኩል በተከታታይ ለ5 ቀናት ከስራ የቀረ ሰራተኛ ከስራ ለማሰናበት አሰሪው ማስጠንቀቂያ መስጠት ይጠበቅበታል ወይ የሚለው ጭብጥ አከራካሪ ነው፡፡ ይህን መሰረት በማድረግ የፌ/ጠ/ፍ/ቤት በሰ/መ/ቁ 199956 በየካቲት 30/2013 ዓ.ም ( ያልታተመ) ውሳኔው ላይ አስገዳጅ የህግ ትርጓሜ ሰጥቷል፡፡ ትርጓሜውም “ ቀደም ሲል በስራ ላይ የነበረውን አዋጅ ቁጥር 377/96 አንቀጽ 27(1)(ለ) ስር";
            if (systemManager != null)
            {
                if (systemManager.CheckCorpus())
                {
                    systemManager.TriggerCorpusPreprocessing();
                }

            }
            else;


            //else Console.WriteLine("SysMan not Init (Correct Init)");
            */