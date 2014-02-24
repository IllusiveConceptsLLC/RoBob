using System;
using System.Collections.Generic;
using System.Xml.Serialization;


[XmlRoot("texture"), System.Serializable]
public class TextureData {
	public string id;
	public int image_count;
	public int width;
	public int height;

	public string getId() {
		return id;
	}
	public int getImage_count() {
		return image_count;
	}
	public int getWidth() {
		return width;
	}
	public int getHeight() {
		return height;
	}

	public void setId(string i) {
		id = i;
	}
	public void setImage_count(int i) {
		image_count = i;
	}
	public void setWidth(int i) {
		width = i;
	}
	public void setHeight(int i) {
		height = i;
	}

	[XmlArray("images"), XmlArrayItem("image")]
	public List<ImageData> images;

	public List<ImageData> getImages() {
		return images;
	}

	public void setImages(List<ImageData> id) {
		images = id;
	}
}