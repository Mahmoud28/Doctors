using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class CreateEventPlayAPIViewModel/* : APIBaseModel*/
    {
        public CreateEventPlayAPIViewModel()
        {
            
        }

        //public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string TheaterName { get; set; }
        public int? StageType { get; set; }
        public string Address { get; set; }
        public int? NumberOfAvailableSeats { get; set; }
        public int? ShowsPerDay { get; set; }
        public string Comments { get; set; }
        public string Descriptions { get; set; }
        public string Type { get; set; }
        public decimal? ToPrice { get; set; }
        public decimal? FromPrice { get; set; }
    
        public long? Date { get; set; }
        public string Time { get; set; }
        public string Duration { get; set; }
      //  public string SeatingChartImage { get; set; }
      //  public string CoverImage { get; set; }
        public string TrailerLink { get; set; }
        //public bool? IsDeleted { get; set; }
      //  public DateTime? CreateDate { get; set; }
        //
        public List<int> Categorys { get; set; }
       // public string CategorysStr { get; set; }
        //public string TypeStr { get; set; }
       // public string StageType { get; set; }
        public IFormFile UploadedCoverImage { get; set; }
       public IFormFile UploadedSeatingChartImage { get; set; }

    
    }
    public static class CategoryEnum
    {
        public const string Comedy = "Comedy";
        public const string Drama = "Drama";
        public const string Action = "Action";
        public const string Advanture = "Advanture";
    }
    public static class StageTypeEnum
    {
        public const string Curved = "Curved";
        public const string Straight = "Straight";
        public const string NoSeat = "No Seat";
    }
    public static class ShowTypeEnum
    {
        public const string Play = "Play";
        public const string Event = "Event";
    }
}
