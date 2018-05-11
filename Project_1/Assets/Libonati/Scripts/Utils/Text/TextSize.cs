using UnityEngine;
using System.Collections;

/**
 
 TextSize for Unity3D by thienhaflash (thienhaflash@gmail.com)
 
 Version	: 0.1
 Update		: 18.Jun.2012
 Features	:
	Return perfect size for any TextMesh
 	Cache the size of each character to speed up the size
	Evaluate and cache only when there are requirements
 
 Sample 	:
		
		//declare it locally, so we can have access anywhere from the script
		TextSize ts;
		
		//put this on the Start function
	 	ts = new TextSize(gameObject.GetComponent<TextMesh>());
		
		//anywhere, after you change the text :
		print(ts.width);
		
		//or get the length of an abitrary text (that is not assign to TextMesh)
		print(ts.GetTextWidth("any abitrary text goes here"));

		//Call this to get a clamped string
		ResolveTextSize("My Text To Clamp", 12);

 You are free to use the code or modify it any way you want (even remove this block of comments) but if it's good
 please give it back to the community.
 
 */

public class TextSize {
	private Hashtable dict; //map character -> width
	
	private TextMesh textMesh;
	private Renderer renderer;
	public int numberOfLines = 1;

	public TextSize(TextMesh tm){
		textMesh = tm;
		renderer = tm.GetComponent<Renderer>();
		dict = new Hashtable();
		getSpace();
	}
	
	private void getSpace(){//the space can not be got alone
		string oldText = textMesh.text;
		
		textMesh.text = "a";
		float aw = renderer.bounds.size.x;
		textMesh.text = "a a";
		float cw = renderer.bounds.size.x - 2* aw;
		
		//MonoBehaviour.print("char< > " + cw);
		dict.Add(' ', cw);
		dict.Add('a', aw);
		
		textMesh.text = oldText;
	}
	
	public float getTextWidth(string s) {
		char[] charList = s.ToCharArray();
		float w = 0;
		char c;
		string oldText = textMesh.text;
		
		for (int i=0; i<charList.Length; i++){
			c = charList[i];
			
			if (dict.ContainsKey(c)){
				w += (float)dict[c];
			} else {
				textMesh.text = ""+c;
				float cw = renderer.bounds.size.x;
				dict.Add(c, cw);
				w += cw;
				//MonoBehaviour.print("char<" + c +"> " + cw);
			}
		}
		
		textMesh.text = oldText;
		return w;
	}
	
	public float width { get { return getTextWidth(textMesh.text); } }
	public float height { get { return renderer.bounds.size.y; } }

	// Wrap text by line height
	public string ResolveTextSize(string input, int lineLength){
		numberOfLines = 1;
		// Split string by char " "    
		string[] words = input.Split(" "[0]);
		
		// Prepare result
		string result = "";
		
		// Temp line string
		string line = "";
		
		// for each all words     
		foreach(string s in words){
			// Append current word into line
			string temp = line + " " + s;
			
			// If line length is bigger than lineLength
			if(temp.Length > lineLength){
				
				// Append current line into result
				result += line + "\n";
				// Remain word append into new line
				line = s;
				if(s.Length>0)
				numberOfLines++;
			}
			// Append current word into current line
			else {
				line = temp;
			}
		}
		
		// Append last line into result   
		result += line;
		
		// Remove first " " char
		return result.Substring(1,result.Length-1);
	}
}