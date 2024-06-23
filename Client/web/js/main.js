var currentMapX = 0;
var currentMapY = 0;

var viewMapX = 0;
var viewMapY = 0;

var offset = 0;
var gl = null;
var colorUniformLocation = null;
var map = null;
//allBody.onkeydown = handle;
allBody.onclick = handleClick;
allBody.ondblclick = handleDblClick;

function start() {
  var canvas = document.getElementById("glcanvas"); 
  gl = canvas.getContext("webgl");
  if (!gl) {
    return;
  }

  // Get the strings for our GLSL shaders
  var vertexShaderSource = document.getElementById("shader-vs").text;
  var fragmentShaderSource = document.getElementById("shader-fs").text;

  // create GLSL shaders, upload the GLSL source, compile the shaders
  var vertexShader = createShader(gl, gl.VERTEX_SHADER, vertexShaderSource);
  var fragmentShader = createShader(gl, gl.FRAGMENT_SHADER, fragmentShaderSource);

  // Link the two shaders into a program
  var program = createProgram(gl, vertexShader, fragmentShader);

  // look up where the vertex data needs to go.
  var positionAttributeLocation = gl.getAttribLocation(program, "a_position");
  var resolutionUniformLocation = gl.getUniformLocation(program, "u_resolution");
  colorUniformLocation = gl.getUniformLocation(program, "u_color");

  // Create a buffer and put three 2d clip space points in it
  var positionBuffer = gl.createBuffer();

  // Bind it to ARRAY_BUFFER (think of it as ARRAY_BUFFER = positionBuffer)
  gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);

  var positions = [
    0, 100,
    200, 300,
    100, 300,
  ];
  gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(positions), gl.STATIC_DRAW);

  // code above this line is initialization code.
  // code below this line is rendering code.

  //webglUtils.resizeCanvasToDisplaySize(gl.canvas);

  // Tell WebGL how to convert from clip space to pixels
  gl.viewport(0, 0, gl.canvas.width, gl.canvas.height);

  // Clear the canvas
  gl.clearColor(0, 0, 0, 0);
  gl.clear(gl.COLOR_BUFFER_BIT);

  // Tell it to use our program (pair of shaders)
  gl.useProgram(program);

  // Turn on the attribute
  gl.enableVertexAttribArray(positionAttributeLocation);

  // Bind the position buffer.
  gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
  
  gl.uniform2f(resolutionUniformLocation, gl.canvas.width, gl.canvas.height);

  // Tell the attribute how to get data out of positionBuffer (ARRAY_BUFFER)
  var size = 2;          // 2 components per iteration
  var type = gl.FLOAT;   // the data is 32bit floats
  var normalize = false; // don't normalize the data
  var stride = 0;        // 0 = move forward size * sizeof(type) each iteration to get the next position  
  gl.vertexAttribPointer(positionAttributeLocation, size, type, normalize, stride, offset);
  map = getMap();  
  drawMap();
  drawCurPosition(); 
}

function handleClick(e) {
	console.log(e.type +' x=' + e.x + ' y=' + e.y);
	var newMapX = Math.floor((e.x - 8)/30);
	var newMapY = Math.floor((e.y - 8)/30);
	if(newMapX >=0 && newMapX < 10 && newMapY >=0 && newMapY < 10) {
		viewMapX = newMapX;
		viewMapY = newMapY;
		drawMap();
		drawCurPosition();
	}
}

function handleDblClick(e) {
	console.log(e.type +' x=' + e.x + ' y=' + e.y);
	var newMapX = Math.floor((e.x - 8)/30);
	var newMapY = Math.floor((e.y - 8)/30);
	if(newMapX >=0 && newMapX < 10 && newMapY >=0 && newMapY < 10 
		&& Math.abs(newMapX-currentMapX)<2 
		&& Math.abs(newMapY-currentMapY)<2
	) 
	{
		
		currentMapX = newMapX;
		currentMapY = newMapY;
		viewMapX = newMapX;
		viewMapY = newMapY;
		drawMap();
		drawCurPosition();
	}
}

function handle(e) {
	// var redraw = false;
	// console.log(e.type +' key=' + e.key + ' code=' + e.code);
	// if(e.code == "ArrowLeft") {		
		// if(currentMapX>0){
			// currentMapX--;
			// redraw = true;
		// }
	// }
	// if(e.code == "ArrowRight") {		
		// if(currentMapX<9) {
			// currentMapX++;
			// redraw = true;
		// }
	// }
	// if(e.code == "ArrowUp") {		
		// if(currentMapY>0) {			
			// currentMapY--;
			// redraw = true;
		// }
	// }
	// if(e.code == "ArrowDown") {		
		// if(currentMapY<9) {
			// currentMapY++;
			// redraw = true;
		// }
	// }
	// if(redraw){
		// drawMap();
        // drawCurPosition();
	// }
}

function drawCurPosition(){
	if(currentMapX == viewMapX && currentMapY == viewMapY)
	{
		drawRect(currentMapX, currentMapY, 0.9, 0.5, 0.5);
	}
	else
	{
		drawRect(currentMapX, currentMapY, 0.9, 0.6, 0.6);
		drawRect(viewMapX, viewMapY, 0.9, 0.7, 0.7);
	}
}

function drawMap(){  
  map.forEach(function(row, index1, array1) {	  
	row.forEach(function(item, index2, array2) {	   
		drawRect(index1, index2, 0.5, 0.5, 0.5);
	  });
  });
  var infoElView = document.getElementById("infoView");  
  var infoElCurrent = document.getElementById("infoCurrent");
  infoElView.innerHTML = "View position terrain type: " + map[viewMapX][viewMapY].Type;
  infoElCurrent.innerHTML = "Current position terrain type: " + map[currentMapX][currentMapY].Type;
}

function drawRect(x, y, red, green, blue){
	var primitiveType = gl.TRIANGLES;
	SetRect(gl, x, y);
    gl.uniform4f(colorUniformLocation, red, green, blue, 1);	
	count = 6;
	gl.drawArrays(primitiveType, offset, count);
}

function getMap(){
	var map = [];
	for(var i = 0; i < 10; i++)
	{
		var row = [];
		for(var j = 0; j < 10; j++)
		{
			row.push(new Terrain(randomInt(10)));
		}		
		map.push(row);
	}
	return map;
}

class Terrain{
	Type = 0;
	constructor(type)
	{
		this.Type = type;
	}
}

function createShader(gl, type, source) {
  var shader = gl.createShader(type);   // создание шейдера
  gl.shaderSource(shader, source);      // устанавливаем шейдеру его программный код
  gl.compileShader(shader);             // компилируем шейдер
  var success = gl.getShaderParameter(shader, gl.COMPILE_STATUS);
  if (success) {                        // если компиляция прошла успешно - возвращаем шейдер
    return shader;
  }
 
  console.log(gl.getShaderInfoLog(shader));
  gl.deleteShader(shader);
}

function createProgram(gl, vertexShader, fragmentShader) {
  var program = gl.createProgram();
  gl.attachShader(program, vertexShader);
  gl.attachShader(program, fragmentShader);
  gl.linkProgram(program);
  var success = gl.getProgramParameter(program, gl.LINK_STATUS);
  if (success) {
    return program;
  }
 
  console.log(gl.getProgramInfoLog(program));
  gl.deleteProgram(program);
}

function randomInt(range) {
  return Math.floor(Math.random() * range);
}

function SetRect(gl, x, y)
{
	setRectangle(gl, x*30+1, y*30+1, 28, 28);	
}

function setRectangle(gl, x, y, width, height) {
  var x1 = x;
  var x2 = x + width;
  var y1 = y;
  var y2 = y + height;
  gl.bufferData(gl.ARRAY_BUFFER, new Float32Array([
     x1, y1,
     x2, y1,
     x1, y2,
     x1, y2,
     x2, y1,
     x2, y2,
  ]), gl.STATIC_DRAW);
}

start();