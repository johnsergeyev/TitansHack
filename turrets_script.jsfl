const dict = {
	"Cannon_GausGun_02": {
		x: -78,
		y: -91
	},
	"ID_54": {
		x: -57,
		y: -63
	}
};

const out_folder = "file:///Volumes/titans_remote/units/";
const in_folder = "file:///Volumes/Data HD/Dropbox/titans-art/Models/Buildings/Compositing/Cannons/ID_54/";

const PNG = ".png";

const MODE_MOVE = "move";
const MODE_MAKE = "make";

const in_prefix = "ID_54";
const out_prefix = "weapon_54";

const frame_extender = 1;
//const current_mode = MODE_MAKE;
const current_mode = MODE_MOVE;

var X;
var Y;
const SCALE_X = 1;
const SCALE_Y = 1;

const directions = [
	"sw", 
	"ssw", "sssw", "ssssw", 
	"s", 
	"sssse", "ssse", "sse", 
	"se", 
	"see", "seee", "seeee", 
	"e", 
	"neeee", "neee", "nee", 
	"ne", 
	"nne", "nnne", "nnnne", 
	"n", 
	"nnnnw", "nnnw", "nnw", 
	"nw",
	"nww", "nwww", "nwwww",
	"w",
	"swwww", "swww", "sww"
];

if (current_mode == MODE_MOVE) {
	
	if (dict.hasOwnProperty(in_prefix)) {
		X = dict[in_prefix].x;
		Y = dict[in_prefix].y;
	}
	
	var docs = fl.documents;
	
	for (var i = 0; i < docs.length; i++) {
		var doc = docs[i];
		var doc_items = doc.library.items;
		var doc_item;
		var timeline;
		var layer;
		var fr;
		var elements;
		
		for (var t = 0; t < doc_items.length; t++) {
			doc_item = doc_items[t];
			
			if (doc_item.name.search(out_prefix) == 0)	{
				fl.trace(doc_item.name + " moved");	
				
				timeline = doc_item.timeline;
				layer = timeline.layers[0];
				
				fr = layer.frames;
				
				for (var j = 0; j < fr.length; j++) {
					elements = fr[j].elements;
					for (var e = 0; e < elements.length; e++) {
						elements[e].x = X;
						elements[e].y = Y;
						elements[e].scaleX = SCALE_X;
						elements[e].scaleY = SCALE_Y;
					}
				}
			}
		}
	}
	
	fl.trace("move complete");	
	
} else {

	var start = Date.now();
	var stop;

	var document = fl.createDocument();

	importSeq(document);
	applyParams(document);
	saveDoc(document);

	stop = Date.now();

	fl.trace("===============================================");
	fl.trace("total time: " + (stop - start)/1000 + " sec");

}

function importSeq(doc) {
	var check = true;
	var current = 0;
	var current_str;
	var p;
	var mdl;
	
	while (current <= 31) {
		p = in_folder + in_prefix + "_";
		current_str = current.toString();
		
		while (current_str.length != 2) {
			current_str = "0" + current_str;
		}
		
		p = p + current_str + PNG;
		
		fl.trace("Import: "+p);
		
		doc.importFile(p, true);
		current++;
	}
	
	var old_name = in_prefix + "_";
	rename(doc, old_name);	
}

function applyParams(doc) {
	var lib = doc.library;
	var current_dir = directions;
 	var items;
	var name;
	var item;
	var timeline;
	var current_str;
	
	var name_png;
	var item_png;
	
	var start_frame;
	var stop_frame;
	
	for (var i = 0; i < current_dir.length; i++) {
		name = out_prefix + "_body_" + current_dir[i];		
		lib.addNewItem("movie clip", name);		
		items = lib.items;
		item = items[lib.findItemIndex(name)];	
		
		timeline = item.timeline;
		timeline.setSelectedLayers(0);		
		
		timeline.setSelectedFrames(0, frame_extender); 
		timeline.removeFrames();
			
		for (var j = 0; j < 1; j++) {
			current_str = (i * 1 + j).toString();		
			
			while (current_str.length != 2) {
				current_str = "0" + current_str;
			}		
			
			name_png = current_str + PNG;
			fl.trace("Place png: "+name_png);	
			
			item_png = items[lib.findItemIndex(name_png)];
			
			start_frame = j * frame_extender;
			stop_frame = (j + 1) * frame_extender; 
			
			timeline.insertKeyframe(start_frame);
			timeline.convertToBlankKeyframes(start_frame);
			timeline.setSelectedFrames(start_frame, start_frame);
			lib.editItem(name);
			doc.addItem({x:0,y:0}, item_png); 
			
			//if (framesPerItem[index] != 1) {
			//	timeline.insertFrames(frame_extender - 1);
			//}
		}
		
		//if (framesPerItem[index] == 1) {
			timeline.removeFrames(1);
		//} else {
		//	timeline.removeFrames(framesPerItem[index] * frame_extender);
		//}
		
		item.linkageExportForAS = true;
		item.linkageClassName = name;
	}
}

function rename(doc, in_str) {
	lib = doc.library;
      if (lib)
      {
        chi = lib.items;
        if (chi)
        {
          cen = chi.length;
          for (var j = 0; j < cen; j++)
          {
            ite = chi[j];
            if (ite)
            {
				
				if (in_str.length > 0)	{
				  //name
				  
				  var links = ["name", "linkageClassName"];
				  var link;
				  
				  for (var i = 0; i < links.length; i++) {
					  link = links[i];

					  if (!ite[link]) continue;
					  
					  if (ite[link].search(in_str) == 0)	{
						str = ite[link].substr(in_str.length);
						//ite[link] = "";
						ite[link] = str;
					  }
				  }
				}
			  
			  //suffix
			  if (PNG.length > 0)	{
				  if (ite.name.search(PNG) >= 0)	{	
					ite.name = ite.name.substr(0, ite.name.length - PNG.length) + PNG;
				  }
			  }
            }
          }
        }
      }
}

function saveDoc(doc) {
	var folder = out_folder + out_prefix + "/";
	var savePath = out_folder + out_prefix + "/" + out_prefix + "_body.fla";
	FLfile.createFolder(folder);
	fl.saveDocument(document, savePath);	
	//fl.closeDocument(document);
}