using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCalendar
{
    class calculation_entry
    {
         private int result=0;
		 private int b;
		
	       public calculation_entry(int result,int b) {
	       this.result = result;
	       this.b = b;
           }
   

   public void writeToFile_addition() {
	         
	    	 result = result+b;
	         String newEvent = result.ToString();
	        	                 
            using (StreamWriter writer = new StreamWriter("Calendar.txt", true))
            {
                writer.WriteLine(newEvent);
            }
	     }
   
    public void writeToFile_subtraction() {
	         
	    	 result = result-b;
	         String newEvent = result.ToString();
	        	                 
            using (StreamWriter writer = new StreamWriter("Calendar.txt", true))
            {
                writer.WriteLine(newEvent);
            }
	     }
        
    public void writeToFile_multiplication() {
	        
	    	 result = result*b;
	         String newEvent = result.ToString();
	        	                 
            using (StreamWriter writer = new StreamWriter("Calendar.txt", true))
            {
                writer.WriteLine(newEvent);
            }
	     }

   public void writeToFile_division() {
	         
	    	 result = result/b;
	         String newEvent = result.ToString();
	        	                 
            using (StreamWriter writer = new StreamWriter("Calendar.txt", true))
            {
                writer.WriteLine(newEvent);
            }
	     }
     public String makeString()
        {
            return (result.ToString());
        }

	 }

}
