/*
 * Canvas World Map function (documentation: http://joncom.be/code/excanvas-world-map)
 *
 * Copyright (c) 2009 Jon Combe (http://joncom.be)
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

 function WorldMap(oSettings) {
  // create variables
  var oSettings = (oSettings || {});
  oSettings.detail = (oSettings.detail || {});
  var sBGColor = (oSettings.bgcolor || "#ffffff");
  var sFGColor = (oSettings.fgcolor || "#dddddd");
  var sBorderColor = (oSettings.bordercolor || "#aaaaaa");
  var iPadding = ((oSettings.padding || 10) * 2);
  var sZoom = (oSettings.zoom || "Friesland,Groningen,Drenthe");
  var iOffsetX = 0;
  var iOffsetY = 0;

  // get canvas dimensions, set bgcolor
  var oCanvas = document.getElementById(oSettings.id);
  if (!oCanvas) {
    alert("Error: missing or incorrect canvas 'id'");
  }
  var iCanvasWidth = oCanvas.width;
  var iCanvasHeight = oCanvas.height;
  oCanvas.style.backgroundColor = sBGColor;

  // create drawing area
  var oCTX = (oCanvas).getContext('2d');
  oCTX.clearRect(0, 0, iCanvasWidth, iCanvasHeight);
  oCTX.lineWidth = (oSettings.borderwidth || 1);

  // calculate zoom: create variables
  var aZoom = sZoom.split(",");
  var iMinX = oWorldMap[aZoom[0]][0][0][0];
  var iMaxX = oWorldMap[aZoom[0]][0][0][0];
  var iMinY = oWorldMap[aZoom[0]][0][0][1];
  var iMaxY = oWorldMap[aZoom[0]][0][0][1];

  // calculate zoom: find map range
  for (var iCountry = 0; iCountry < aZoom.length; iCountry++) {
    for (var iPath = 0; iPath < oWorldMap[aZoom[iCountry]].length; iPath++) {
      for (iCoord = 0; iCoord < oWorldMap[aZoom[iCountry]][iPath].length; iCoord++) {
        iMinX = Math.min( iMinX, oWorldMap[aZoom[iCountry]][iPath][iCoord][0] );
        iMaxX = Math.max( iMaxX, oWorldMap[aZoom[iCountry]][iPath][iCoord][0] );
        iMinY = Math.min( iMinY, oWorldMap[aZoom[iCountry]][iPath][iCoord][1] );
        iMaxY = Math.max( iMaxY, oWorldMap[aZoom[iCountry]][iPath][iCoord][1] );
      }
    }
  }

  // calculate zoom ratio
  var iRatio = Math.min( ((iCanvasWidth - iPadding) / (iMaxX - iMinX)), ((iCanvasHeight - iPadding) / (iMaxY - iMinY)) );

  // calculate zoom offsets
  var iMidX = (iMinX + ((iMaxX - iMinX) / 2));
  var iMidY = (iMinY + ((iMaxY - iMinY) / 2));
  iOffsetX = ((iMidX * iRatio) - (iCanvasWidth / 2));
  iOffsetY = ((iMidY * iRatio) - (iCanvasHeight / 2));

  // draw "plain" countries
  for (var sCountry in oWorldMap) {
    Draw(sCountry, sFGColor);
  }

  // draw "details" countries
  for (var sCountry in oSettings.detail) {
    if (oWorldMap[sCountry]) {
      Draw(sCountry, oSettings.detail[sCountry]);
    }
  }

  // private draw function
  function Draw(sCountry, sColor) {
    oCTX.fillStyle = sColor;
    oCTX.strokeStyle = sBorderColor;
    oCTX.beginPath();

    // loop through paths
    var bIE = (navigator.userAgent.indexOf("MSIE") > -1);
    for (var iPath = 0; iPath < oWorldMap[sCountry].length; iPath++) {
      oCTX.moveTo((oWorldMap[sCountry][iPath][0][0] * iRatio) - iOffsetX, (oWorldMap[sCountry][iPath][0][1] * iRatio) - iOffsetY);
      for (iCoord = 1; iCoord < oWorldMap[sCountry][iPath].length; iCoord++) {
        oCTX.lineTo((oWorldMap[sCountry][iPath][iCoord][0] * iRatio) - iOffsetX, (oWorldMap[sCountry][iPath][iCoord][1] * iRatio) - iOffsetY);
      }
      oCTX.closePath();
      oCTX.fill();

      // IE, again...
      if (bIE == true) {
        oCTX.beginPath();
        oCTX.moveTo((oWorldMap[sCountry][iPath][0][0] * iRatio) - iOffsetX, (oWorldMap[sCountry][iPath][0][1] * iRatio) - iOffsetY);
        for (iCoord = 1; iCoord < oWorldMap[sCountry][iPath].length; iCoord++) {
          oCTX.lineTo((oWorldMap[sCountry][iPath][iCoord][0] * iRatio) - iOffsetX, (oWorldMap[sCountry][iPath][iCoord][1] * iRatio) - iOffsetY);
        }
        oCTX.closePath();
      }
      oCTX.stroke();
    }
  }
}

var oWorldMap={"Friesland":[[[361,173],[353,168],[345,168],[347,174],[342,175],[336,161],[323,160],[329,177],[316,169],[315,139],[320,129],[307,133],[297,130],[278,132],[251,138],[218,148],[140,191],[118,195],[109,203],[106,210],[101,216],[88,227],[76,231],[58,255],[56,267],[56,283],[50,305],[40,314],[39,323],[52,338],[47,342],[32,325],[48,357],[50,362],[47,377],[54,390],[54,403],[46,403],[51,417],[51,425],[32,435],[35,446],[42,455],[53,460],[64,459],[75,456],[88,461],[92,467],[106,469],[111,469],[116,459],[125,461],[136,454],[140,464],[152,464],[161,462],[162,468],[169,466],[176,468],[182,475],[187,486],[195,485],[201,480],[210,486],[217,486],[224,481],[234,470],[242,470],[248,465],[266,476],[272,477],[278,475],[282,469],[283,463],[296,463],[309,458],[325,448],[335,440],[342,425],[351,417],[362,412],[375,420],[382,424],[394,420],[410,410],[418,396],[418,384],[415,379],[402,369],[390,357],[388,345],[389,334],[380,328],[372,324],[371,316],[355,305],[346,301],[329,303],[321,295],[319,283],[322,272],[335,255],[341,241],[335,229],[345,222],[352,203],[361,195],[361,187]]],"Groningen":[[[371,316],[355,305],[346,301],[329,303],[321,295],[319,283],[322,272],[335,255],[341,241],[335,229],[345,222],[352,203],[361,195],[361,187],[361,173],[367,174],[373,186],[379,188],[384,182],[376,178],[371,171],[364,162],[353,161],[345,163],[347,154],[346,149],[339,152],[332,150],[324,137],[331,136],[339,143],[346,140],[337,135],[333,130],[345,127],[360,134],[377,135],[388,128],[413,120],[437,119],[460,115],[475,109],[491,98],[507,93],[531,98],[546,95],[559,104],[562,110],[567,134],[575,156],[584,168],[601,179],[619,186],[640,188],[640,206],[646,214],[654,219],[665,224],[684,226],[687,233],[688,247],[688,259],[678,285],[686,300],[692,327],[694,346],[693,358],[682,377],[684,391],[683,400],[668,425],[656,432],[652,466],[638,447],[625,442],[628,428],[636,421],[635,416],[621,411],[613,394],[596,373],[583,363],[573,351],[562,341],[549,329],[546,321],[536,312],[512,298],[504,304],[499,300],[491,302],[484,300],[477,291],[468,278],[455,267],[446,256],[434,253],[424,249],[411,260],[401,266],[392,284],[386,305],[378,319]]],"Drenthe":[[[371,316],[372,324],[380,328],[389,334],[388,345],[390,357],[402,369],[415,379],[418,384],[418,396],[410,410],[394,420],[382,424],[375,420],[362,412],[351,417],[342,425],[335,440],[325,448],[309,458],[331,479],[337,490],[336,497],[330,503],[317,509],[313,514],[313,526],[319,545],[324,557],[330,560],[335,557],[341,555],[349,561],[358,567],[370,565],[379,568],[389,575],[403,595],[409,595],[414,587],[422,589],[425,596],[430,593],[434,588],[444,595],[450,593],[447,576],[458,566],[463,562],[474,560],[489,561],[500,566],[510,571],[516,583],[521,585],[534,569],[545,570],[556,573],[562,574],[574,567],[582,570],[596,579],[604,578],[619,572],[632,579],[637,576],[641,566],[641,548],[644,538],[646,524],[642,503],[644,490],[645,474],[652,466],[638,447],[625,442],[628,428],[636,421],[635,416],[621,411],[613,394],[596,373],[583,363],[573,351],[562,341],[549,329],[546,321],[536,312],[512,298],[504,304],[499,300],[491,302],[484,300],[477,291],[468,278],[455,267],[446,256],[434,253],[424,249],[411,260],[401,266],[392,284],[386,305],[378,319]]]}