using UnityEngine;
using System.Collections;

//Modified version of a midpoint displacement algorithm, originally by:

//"Plasma Fractal" by Giles Whitaker, licensed under Creative Commons Attribution-Share Alike 3.0 and GNU GPL license.
//	Work: http://openprocessing.org/visuals/?visualID= 8528
//		License:
//		http://creativecommons.org/licenses/by-sa/3.0/
//		http://creativecommons.org/licenses/GPL/2.0/

//Converted to C# by 'mgear' (http://unitycoder.com/blog/2012/04/03/diamond-square-algorithm/)

//Converted to a static class for better useability by 'Jan Ott' (http://malen-mit-zahlen.blogspot.de/)

public class MidPointDisplacementGenerator : MonoBehaviour {
	
	//Offset float value based on grain and map size;
	static float OffsetValue(float currentSum, float totalSum, float grain)
	{
		float max = currentSum / totalSum * grain;
		return Random.Range(-0.5f, 0.5f)* max;
	}
	
	//Returns a quadratic 2D array with a length of _length, containing float values between 0f and 1f.
	//Low values for grain (like 2.0f) will result in very smooth maps with little detail, while higher values (like 12.0f) will return very coarse and noisy maps.
	//Every unique _seed integer value will result in a unique map.
	public static float[,] GenerateHeightMap(int _length, float _grain, int _seed)
	{
		float[,] _heightmap = new float[_length, _length];
		
		Random.seed = _seed;
		
		float c1, c2, c3, c4;
		
		//Assign the four corners of the intial grid random color values
		//These will end up being the colors of the four corners of the applet.     
		c1 = Mathf.Lerp (Random.value, 0.5f, 0.5f);
		c2 = Mathf.Lerp (Random.value, 0.5f, 0.5f);
		c3 = Mathf.Lerp (Random.value, 0.5f, 0.5f);
		c4 = Mathf.Lerp (Random.value, 0.5f, 0.5f);
		
		DivideGrid(0.0f, 0.0f, _length, _length , c1, c2, c3, c4, ref _heightmap, _grain, (float)(_length + _length));
		
		return _heightmap;
	}
	
	//This is the recursive function that implements the random midpoint
	//displacement algorithm.  It will call itself until the grid pieces
	//become smaller than one pixel.   
	static void DivideGrid(float x, float y, float _width, float _height, float c1, float c2, float c3, float c4, ref float[,] _array, float _grain, float _totalSum)
	{
		//Half the width and height;
		float newWidth = _width * 0.5f;
		float newHeight = _height * 0.5f;
		
		if (_width < 1.0f || _height < 1.0f)
		{
			//The four corners of the grid piece will be averaged and drawn as a single pixel.
			float c = (c1 + c2 + c3 + c4) * 0.25f;
			_array[(int)x, (int)y] = c;
		}
		else
		{
			float middle =(c1 + c2 + c3 + c4) * 0.25f + OffsetValue(newWidth + newHeight, _totalSum, _grain );      //Randomly displace the midpoint!
			float edge1 = (c1 + c2) * 0.5f; //Calculate the edges by averaging the two corners of each edge.
			float edge2 = (c2 + c3) * 0.5f;
			float edge3 = (c3 + c4) * 0.5f;
			float edge4 = (c4 + c1) * 0.5f;
			
			//Make sure that the midpoint doesn't accidentally "randomly displace" past the boundaries!
			if (middle <= 0)
			{
				middle = 0;
			}
			else if (middle > 1.0f)
			{
				middle = 1.0f;
			}
			
			//Do the operation again for each of the four new grids.                 
			DivideGrid(x, y, newWidth, newHeight, c1, edge1, middle, edge4, ref _array, _grain, _totalSum);
			DivideGrid(x + newWidth, y, newWidth, newHeight, edge1, c2, edge2, middle, ref _array, _grain, _totalSum);
			DivideGrid(x + newWidth, y + newHeight, newWidth, newHeight, middle, edge2, c3, edge3, ref _array, _grain, _totalSum);
			DivideGrid(x, y + newHeight, newWidth, newHeight, edge4, middle, edge3, c4, ref _array, _grain, _totalSum);
		}
	}
}
