
using System;
namespace CSharpDiscovery.Quest03{

  public class PointOfInterest{
          public double Latitude { get; set; } 
          public double Longitude { get; set; }
          public string Name { get; set; } 
          
        public static string GoogleMapsUrlTemplate
    {
        get { return "https://www.google.com/maps/place/{0}/@{1},{2},15z/"; }
        
    }

    public PointOfInterest(){
        Name = "Bordeaux Ynov Campus";
        Latitude = 44.854186;
        Longitude = -0.5663056;
    }

    public PointOfInterest(string Name, double Latitude, double Longitude){
        this.Name = Name;
        this.Latitude = Latitude;
        this.Longitude = Longitude; 
    
    }

    public string GetGoogleMapsUrl(){
        string result = string.Format(GoogleMapsUrlTemplate, Name.Replace(" ", "+" ), Latitude, Longitude);
        return result;
       
    }

    public override string ToString(){
        
        return this.Name + "" + " "+ "(" + "Lat="+this.Latitude + "," + " " + "Long=" + this.Longitude + ")" ;
        
    }

     public int GetDistance(PointOfInterest other)
        {
            double lat1 = Math.PI*Latitude/180;
            double lat2 = Math.PI*other.Latitude/180;
            double theta = Longitude - other.Longitude;
            double rtheta = Math.PI*theta/180;
            double dist = Math.Sin(lat1)*Math.Sin(lat2) + Math.Cos(lat1)*Math.Cos(lat2)*Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist*180/Math.PI;
            dist = dist*60*1.1515;
            dist *= 1.609344;
            int result = (int)Math.Ceiling(dist);
            return result;

        }

        public static int GetDistance(PointOfInterest p1, PointOfInterest p2)
        {
            double lat1 = Math.PI*p1.Latitude/180;
            double lat2 = Math.PI*p2.Latitude/180;
            double theta = p1.Longitude - p2.Longitude;
            double rtheta = Math.PI*theta/180;
            double dist = Math.Sin(lat1)*Math.Sin(lat2) + Math.Cos(lat1)*Math.Cos(lat2)*Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist*180/Math.PI;
            dist = dist*60*1.1515;
            dist *= 1.609344;
            int result = (int)Math.Ceiling(dist);
            return result;

        }

  }

  
    

}