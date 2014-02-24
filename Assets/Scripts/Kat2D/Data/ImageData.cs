using System;
using System.Xml.Serialization;


[XmlRoot("image"), System.Serializable]
public class ImageData {
	public int index;
	public int origin_x;
	public int origin_y;
	public int width;
	public int height;

	public int getIndex() {
		return index;
	}
	public int getOrigin_x() {
		return origin_x;
	}
	public int getOrigin_y() {
		return origin_y;
	}
	public int getWidth() {
		return width;
	}
	public int getHeight() {
		return height;
	}

	public void setIndex(int i) {
		index = i;
	}
	public void setOrigin_x(int i) {
		origin_x = i;
	}
	public void setOrigin_y(int i) {
		origin_y = i;
	}
	public void setWidth(int i) {
		width = i;
	}
	public void setHeight(int i) {
		height = i;
	}
}