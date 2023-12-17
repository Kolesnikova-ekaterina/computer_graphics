#include <GL/glew.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <glm/gtc/type_ptr.hpp>
#include <SOIL/SOIL.h> 
#include <SFML/OpenGL.hpp>
#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>
#include <iostream>
#include <vector>

#include "model.h"

enum flags_axis {
	OX,
	OY,
	OZ,
	NUL
};

flags_axis ax = NUL;

enum flags_dir_or_pos {
	d,
	p,
};

flags_dir_or_pos repl = p;

enum flags_type_sh {
	fong,
	toon,
	rim
};

flags_type_sh sh = fong;


enum flags_type_light {
	dir,
	point,
	spot
};

flags_type_light type_light = dir;

glm::mat4 projection = glm::perspective(glm::radians(45.0f), 900.0f / 900.0f, 0.1f, 100.0f);

//камера
glm::vec3 cameraPos = glm::vec3(0.0f, 4.0f, 35.0f);
glm::vec3 cameraFront = glm::vec3(0.0f, 0.0f, -1.0f);
glm::vec3 cameraUp = glm::vec3(0.0f, 1.0f, 0.0f);


//вращение
bool firstMouse = true;
float yaw = -90.0f;
float pitch = 0.0f;
float last_x = 450.0f;
float last_y = 450.0f;

// ID шейдерной программы
GLuint ProgramRim;
GLuint ProgramToon;
GLuint ProgramFong;

//позиция свта
//glm::vec3 lightPos(-3.0f, 8.0f, 4.0f);
glm::vec3 lightPos(0.0f, 2.0f, 10.0f);
glm::vec3 lightDirection(0.0f, 0.0f, -1.0f);
glm::vec3 lightness(1.0f, 1.0f, 1.0f);
float conus = 12.5f;

//fong
const char* VertexShaderFong = R"(
    #version 330 core

    layout (location = 0) in vec3 coord_pos;
    layout (location = 1) in vec3 normal_in;
    layout (location = 2) in vec2 tex_coord_in;

    out vec2 coord_tex;
	out vec3 normal;
	out vec3 frag_pos;

    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;
    
    void main() 
    { 
        gl_Position = projection * view * model * vec4(coord_pos, 1.0);
        coord_tex = tex_coord_in;
		frag_pos = vec3(model * vec4(coord_pos, 1.0));
		normal =  mat3(transpose(inverse(model))) * normal_in;
        //coord_tex = vec2(tex_coord_in.x, 1.0f - tex_coord_in.y); //если текстуры ннеправильно наложились
    }
    )";


//fong
const char* FragShaderFong = R"(
    #version 330 core

	struct Light {
		vec3 position;
		vec3 direction; //dir and spot
  
		vec3 ambient;
		vec3 diffuse;
		vec3 specular;

	//point
		float constant;
		float linear;
		float quadratic;

	//spot
		float cutOff;
		float outerCutOff;
	};

	uniform Light light;  

    in vec2 coord_tex;
    in vec3 frag_pos;
    in vec3 normal;

	out vec4 frag_color;

    uniform sampler2D texture_diffuse1;
	uniform vec3 viewPos;
	uniform int shininess;
	uniform int type_light;

    void main()  
    {
		vec3 norm = normalize(normal);
		vec3 lightDir;

		if(type_light == 0)
		{
			lightDir = normalize(-light.direction);  //dir
		}
		else
		{
			lightDir = normalize(light.position - frag_pos);  //point and spot
		}
		
		vec3 ambient = light.ambient * texture(texture_diffuse1, coord_tex).rgb; 

		float diff = max(dot(norm, lightDir), 0.0);
		vec3 diffuse = light.diffuse * (diff * texture(texture_diffuse1, coord_tex).rgb); 

		vec3 viewDir = normalize(viewPos - frag_pos);
		vec3 reflectDir = reflect(-lightDir, norm);  

		float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
		vec3 specular = light.specular * (spec * texture(texture_diffuse1, coord_tex).rgb); 

		if(type_light == 1 || type_light == 2)
		{
			//point and spot
				float distance = length(light.position - frag_pos);
				float attenuation = 1.0 / (light.constant + light.linear * distance 
									+ light.quadratic * (distance * distance));
				if(type_light == 1)
				{
					ambient *= attenuation; //point
				}
				diffuse *= attenuation;
				specular *= attenuation;   
			//end point and spot

				if(type_light == 2)
				{	//spot
						float theta = dot(lightDir, normalize(-light.direction)); 
						float epsilon   = light.cutOff - light.outerCutOff;
						float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

						diffuse *= intensity;
						specular *= intensity;
					//end spot
				}
		}

		vec3 result = (ambient + diffuse + specular);
		frag_color = vec4(result, 1.0);
    } 
)";


//toon
const char* VertexShaderToon = R"(
    #version 330 core

    layout (location = 0) in vec3 coord_pos;
    layout (location = 1) in vec3 normal_in;
    layout (location = 2) in vec2 tex_coord_in;

    out vec2 coord_tex;
	out vec3 normal;
	out vec3 frag_pos;

    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;
    
    void main() 
    { 
        gl_Position = projection * view * model * vec4(coord_pos, 1.0);
        coord_tex = tex_coord_in;
		frag_pos = vec3(model * vec4(coord_pos, 1.0));
		normal =  mat3(transpose(inverse(model))) * normal_in;

		
        //coord_tex = vec2(tex_coord_in.x, 1.0f - tex_coord_in.y); //если текстуры ннеправильно наложились
    }
    )";


//toon
const char* FragShaderToon = R"(
    #version 330 core

    struct Light {
		vec3 position;
		vec3 direction; //dir and spot

	//point
		float constant;
		float linear;
		float quadratic;

	//spot
		float cutOff;
	};

	uniform Light light; 
    in vec2 coord_tex;
    in vec3 frag_pos;
    in vec3 normal;

	out vec4 frag_color;

    uniform sampler2D texture_diffuse1;
	uniform vec3 viewPos;
	uniform int type_light;

    void main()  
    {
		vec3 lightDir;
		
		if(type_light == 0)
		{
			lightDir = normalize(-light.direction);  //dir
		}
		else
		{
			lightDir = normalize(light.position - frag_pos);  //point and spot
		}

		vec3 norm = normalize(normal);
		float toon_intensity = max(dot(norm, lightDir), 0.0);

		vec3 result;
		
		if (toon_intensity > 0.95)
			result = vec3(1.0, 1.0, 1.0) * texture(texture_diffuse1, coord_tex).rgb;
		else if (toon_intensity > 0.5)
			result = vec3(0.8, 0.8, 0.8) * texture(texture_diffuse1, coord_tex).rgb;
		else if (toon_intensity > 0.25)
			result = vec3(0.5, 0.5, 0.5) * texture(texture_diffuse1, coord_tex).rgb;
		else
			result = vec3(0.2, 0.2, 0.2) * texture(texture_diffuse1, coord_tex).rgb;

		float theta = dot(lightDir, normalize(-light.direction)); 

		if(type_light == 1 || type_light == 2)
		{
			//point and spot
				float distance    = length(light.position - frag_pos);
				float attenuation = 1.0 / (light.constant + light.linear * distance 
									+ light.quadratic * (distance * distance));
				result *= attenuation;   
			//end point and spot

				if(type_light == 2 && theta <= light.cutOff)
				{	
					//spot
						result = vec3(0.2, 0.2, 0.2) * texture(texture_diffuse1, coord_tex).rgb;	
					//end spot
				}	
		}

		frag_color = vec4(result, 1.0);
    } 
)";


//rim
const char* VertexShaderRim = R"(
    #version 330 core

    layout (location = 0) in vec3 coord_pos;
    layout (location = 1) in vec3 normal_in;
    layout (location = 2) in vec2 tex_coord_in;

    out vec2 coord_tex;
	out vec3 normal;
	out vec3 frag_pos;

    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;
    
    void main() 
    { 
        gl_Position = projection * view * model * vec4(coord_pos, 1.0);
        coord_tex = tex_coord_in;
		frag_pos = vec3(model * vec4(coord_pos, 1.0));
		normal =  mat3(transpose(inverse(model))) * normal_in;

		
        //coord_tex = vec2(tex_coord_in.x, 1.0f - tex_coord_in.y); //если текстуры ннеправильно наложились
    }
    )";

//rim
const char* FragShaderRim = R"(
    #version 330 core

	struct Light {
		vec3 position;
		vec3 direction; //dir and spot
  
		vec3 ambient;
		vec3 diffuse;
		vec3 specular;

	//point
		float constant;
		float linear;
		float quadratic;

	//spot
		float cutOff;
		float outerCutOff;
	};

	uniform Light light;

    in vec2 coord_tex;
    in vec3 frag_pos;
    in vec3 normal;
    in float intensity;

	out vec4 frag_color;

    uniform sampler2D texture_diffuse1;
	uniform vec3 viewPos;
	uniform int shininess;
	uniform int type_light;

    void main()  
    {
		vec3 lightDir;

		if(type_light == 0)
		{
			lightDir = normalize(-light.direction);  //dir
		}
		else
		{
			lightDir = normalize(light.position - frag_pos);  //point and spot
		}

		vec3 viewDir = normalize (viewPos  - frag_pos); 
		vec3 halfwayDir  = normalize (lightDir + viewDir);
		vec3 norm = normalize(normal);

		vec3  specColor = vec3(0.0, 1.0, 0.7);
		//float specPower = 40.0;
		float rimPower = 8.0;
		float bias = 0.5;

		float diff = max(dot(norm, lightDir), 0.0);
		vec3  diffuse = diff * texture(texture_diffuse1, coord_tex).rgb;

		float spec = pow(max(dot(norm, halfwayDir), 0.0), shininess);
		vec3  specular = spec * specColor;
		float rim  = pow(1.0 + bias - max(dot(norm, viewDir), 0.0), rimPower);

		if(type_light == 1 || type_light == 2)
		{
			//point and spot
				float distance    = length(light.position - frag_pos);
				float attenuation = 1.0 / (light.constant + light.linear * distance 
									+ light.quadratic * (distance * distance));
				
				diffuse *= attenuation;
				specular *= attenuation;   
			//end point and spot

			if(type_light == 2)
			{	//spot
					float theta = dot(lightDir, normalize(-light.direction)); 
					float epsilon   = light.cutOff - light.outerCutOff;
					float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

					diffuse *= intensity;
					specular *= intensity;
				//end spot
			}
		}
		
		vec3 result = diffuse + rim * vec3(0.5, 0.0, 0.2) + specular * specColor;
		frag_color = vec4(result, 1.0);
		
    } 
)";




/*

 void main()
	{
		float ambientStrength = 0.1;
		vec3 ambient = light.ambient * texture(texture_diffuse1, coord_tex).rgb; //ambientStrength * lightColor;

		vec3 norm = normalize(normal);
		vec3 lightDir = normalize(lightPos - frag_pos);

		float diff = max(dot(norm, lightDir), 0.0);
		vec3 diffuse = light.diffuse * (diff * texture(texture_diffuse1, coord_tex).rgb); // * lightColor;

		//float specularStrength = 0.5;
		vec3 viewDir = normalize(viewPos - frag_pos);
		vec3 reflectDir = reflect(-lightDir, norm);

		float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
		//vec3 specular = specularStrength * (spec * texture(texture_diffuse1, coord_tex).rgb); // * lightColor;
		vec3 specular = light.specular * (spec * texture(texture_diffuse1, coord_tex).rgb); // * lightColor;

		vec3 result = (ambient + diffuse + specular);
		frag_color = vec4(result, 1.0);

		//gl_FragColor = texture(texture_diffuse1, coord_tex);
		//gl_FragColor = vec4(1.0,0.0,0.0,1.0);
	}



*/

void checkOpenGLerror()
{
	GLenum err = glGetError();
	if (err != GL_NO_ERROR)
	{
		std::cout << "OpenGL error " << err << std::endl;
	}
}

void ShaderLog(unsigned int shader)
{
	int infologLen = 0;
	glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &infologLen);
	GLint vertex_compiled;
	glGetShaderiv(shader, GL_COMPILE_STATUS, &vertex_compiled);

	if (infologLen > 1)
	{
		int charsWritten = 0;
		std::vector<char> infoLog(infologLen);
		glGetShaderInfoLog(shader, infologLen, &charsWritten, infoLog.data());
		std::cout << "InfoLog: " << infoLog.data() << std::endl;
	}

	if (vertex_compiled != GL_TRUE)
	{
		GLsizei log_length = 0;
		GLchar message[1024];
		glGetShaderInfoLog(shader, 1024, &log_length, message);
		std::cout << "InfoLog2: " << message << std::endl;
	}

}

void InitShader()
{
	GLuint vShaderRim = glCreateShader(GL_VERTEX_SHADER);
	// Передаем исходный код
	glShaderSource(vShaderRim, 1, &VertexShaderRim, NULL);
	// Компилируем шейдер
	glCompileShader(vShaderRim);
	std::cout << "vertex shader r\n";
	// Функция печати лога шейдера
	ShaderLog(vShaderRim);

	GLuint vShaderToon = glCreateShader(GL_VERTEX_SHADER);
	// Передаем исходный код
	glShaderSource(vShaderToon, 1, &VertexShaderToon, NULL);
	// Компилируем шейдер
	glCompileShader(vShaderToon);
	std::cout << "vertex shader t\n";
	// Функция печати лога шейдера
	ShaderLog(vShaderToon);

	GLuint vShaderFong = glCreateShader(GL_VERTEX_SHADER); 
	// Передаем исходный код
	glShaderSource(vShaderFong, 1, &VertexShaderFong, NULL); 
	// Компилируем шейдер
	glCompileShader(vShaderFong);
	std::cout << "vertex shader f\n";
	// Функция печати лога шейдера
	ShaderLog(vShaderFong); 

	//-----------------------

	// Создаем фрагментный шейдер
	GLuint fShaderRim = glCreateShader(GL_FRAGMENT_SHADER); 
	// Передаем исходный код
	glShaderSource(fShaderRim, 1, &FragShaderRim, NULL); 
	// Компилируем шейдер
	glCompileShader(fShaderRim); 
	std::cout << "fragment shader r\n";
	// Функция печати лога шейдера
	ShaderLog(fShaderRim);
	
	// Создаем фрагментный шейдер
	GLuint fShaderToon = glCreateShader(GL_FRAGMENT_SHADER);
	// Передаем исходный код
	glShaderSource(fShaderToon, 1, &FragShaderToon, NULL);
	// Компилируем шейдер
	glCompileShader(fShaderToon);
	std::cout << "fragment shader t \n";
	// Функция печати лога шейдера
	ShaderLog(fShaderToon);
	
	// Создаем фрагментный шейдер
	GLuint fShaderFong = glCreateShader(GL_FRAGMENT_SHADER);
	// Передаем исходный код
	glShaderSource(fShaderFong, 1, &FragShaderFong, NULL);
	// Компилируем шейдер
	glCompileShader(fShaderFong);
	std::cout << "fragment shader f\n";
	// Функция печати лога шейдера
	ShaderLog(fShaderFong);

	ProgramRim = glCreateProgram();
	glAttachShader(ProgramRim, vShaderRim);
	glAttachShader(ProgramRim, fShaderRim);

	// Линкуем шейдерную программу
	glLinkProgram(ProgramRim);

	// Проверяем статус сборки
	int link_ok;
	glGetProgramiv(ProgramRim, GL_LINK_STATUS, &link_ok);

	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}

	ProgramToon = glCreateProgram();
	glAttachShader(ProgramToon, vShaderToon);
	glAttachShader(ProgramToon, fShaderToon);

	// Линкуем шейдерную программу
	glLinkProgram(ProgramToon);

	// Проверяем статус сборки
	glGetProgramiv(ProgramToon, GL_LINK_STATUS, &link_ok);

	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}
	

	ProgramFong = glCreateProgram();
	glAttachShader(ProgramFong, vShaderFong);
	glAttachShader(ProgramFong, fShaderFong);

	// Линкуем шейдерную программу
	glLinkProgram(ProgramFong);

	// Проверяем статус сборки
	glGetProgramiv(ProgramFong, GL_LINK_STATUS, &link_ok);

	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}

	checkOpenGLerror();
}




void Init()
{
	// Шейдеры
	InitShader();

	//включаем тест глубины
	glEnable(GL_DEPTH_TEST);
}



void Draw(sf::Clock clock, vector<Model> modelka, GLint shader, int count)
{
	glm::mat4 view = glm::mat4(1.0f);
	glm::mat4 model = glm::mat4(1.0f);

	glUseProgram(shader); // Устанавливаем шейдерную программу текущей

	view = glm::lookAt(cameraPos, cameraPos + cameraFront, cameraUp);

	projection = glm::perspective(glm::radians(45.0f), 900.0f / 900.0f, 0.1f, 100.0f);
	
	glUniform3f(glGetUniformLocation(shader, "light.position"), lightPos.x, lightPos.y, lightPos.z);
	//glUniform3f(glGetUniformLocation(shader, "light.position"), cameraPos.x, cameraPos.y, cameraPos.z);
	//glUniform3f(glGetUniformLocation(shader, "light.position"), 0.0f, 4.0f, 10.0f);
	
	glUniform3f(glGetUniformLocation(shader, "viewPos"), cameraPos.x, cameraPos.y, cameraPos.z);
	
	glUniform1i(glGetUniformLocation(shader, "type_light"), type_light);

	glUniform3f(glGetUniformLocation(shader, "light.ambient"), 0.2f, 0.2f, 0.2f);
	glUniform3f(glGetUniformLocation(shader, "light.diffuse"), 0.9f, 0.9f, 0.9);
	glUniform3f(glGetUniformLocation(shader, "light.specular"), 1.0f, 1.0f, 1.0f);
	glUniform1i(glGetUniformLocation(shader, "shininess"), 16);

	if(type_light == dir)
		//glUniform3f(glGetUniformLocation(shader, "light.direction"), -2.0f, -2.0f, -2.0f);
		glUniform3f(glGetUniformLocation(shader, "light.direction"), lightDirection.x, lightDirection.y, lightDirection.z);
	//if(type_light == spot)
	//	glUniform3f(glGetUniformLocation(shader, "light.direction"), cameraFront.x, cameraFront.y, cameraFront.z);
	else
		glUniform3f(glGetUniformLocation(shader, "light.direction"), lightDirection.x, lightDirection.y, lightDirection.z);
	
	glUniform1f(glGetUniformLocation(shader, "light.constant"), 1.0f);
	glUniform1f(glGetUniformLocation(shader, "light.linear"), 0.045f);
	glUniform1f(glGetUniformLocation(shader, "light.quadratic"), 0.0075f);

	glUniform1f(glGetUniformLocation(shader, "light.cutOff"), glm::cos(glm::radians(conus)));
	glUniform1f(glGetUniformLocation(shader, "light.outerCutOff"), glm::cos(glm::radians(conus * 1.4f)));

	glUniformMatrix4fv(glGetUniformLocation(shader, "view"), 1, GL_FALSE, glm::value_ptr(view));
	glUniformMatrix4fv(glGetUniformLocation(shader, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
	glUniformMatrix4fv(glGetUniformLocation(shader, "model"), 1, GL_FALSE, glm::value_ptr(model));

	float angle = 90.0f;

	model = glm::scale(model, glm::vec3(1.0f, 1.0f, 1.0f));

	model = glm::translate(model, glm::vec3(0.0f, 1.0f, 0.0f));

	model = glm::rotate(model, glm::radians(angle), glm::vec3(1.0f, 0.0f, 0.0f));

	glUniformMatrix4fv(glGetUniformLocation(shader, "model"), 1, GL_FALSE, glm::value_ptr(model));

	modelka[0].Draw(shader, count);


	model = glm::mat4(1.0f);

	model = glm::translate(model, glm::vec3(4.0f, 0.0f, 2.0f));
	
	glUniformMatrix4fv(glGetUniformLocation(shader, "model"), 1, GL_FALSE, glm::value_ptr(model));
	
	modelka[1].Draw(shader, count);

	model = glm::mat4(1.0f);

	model = glm::translate(model, glm::vec3(4.0f, 1.4f, 2.0f));

	glUniformMatrix4fv(glGetUniformLocation(shader, "model"), 1, GL_FALSE, glm::value_ptr(model));

	modelka[2].Draw(shader, count);

	model = glm::mat4(1.0f);

	model = glm::translate(model, glm::vec3(2.5f, 0.0f, 3.0f));

	glUniformMatrix4fv(glGetUniformLocation(shader, "model"), 1, GL_FALSE, glm::value_ptr(model));

	modelka[3].Draw(shader, count);
	
	model = glm::mat4(1.0f);

	model = glm::translate(model, glm::vec3(2.0f, 0.0f, 2.0f));

	glUniformMatrix4fv(glGetUniformLocation(shader, "model"), 1, GL_FALSE, glm::value_ptr(model));

	modelka[4].Draw(shader, count);

	glUseProgram(0); // Отключаем шейдерную программу


	checkOpenGLerror();
}


// Освобождение буфера
void ReleaseVBO()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
}

// Освобождение шейдеров
void ReleaseShader()
{
	// Передавая ноль, мы отключаем шейдерную программу
	glUseProgram(0);
	// Удаляем шейдерные программы
	glDeleteProgram(ProgramToon);
	glDeleteProgram(ProgramRim);
	glDeleteProgram(ProgramFong);
}


void Release()
{
	// Шейдеры
	ReleaseShader();
	// Вершинный буфер
	ReleaseVBO();
}


int main()
{
	std::setlocale(LC_ALL, "Russian");

	sf::Window window(sf::VideoMode(900, 900), "Fill", sf::Style::Default, sf::ContextSettings(24));
	window.setVerticalSyncEnabled(true);
	window.setActive(true);
	
	//window.setMouseCursorGrabbed(true); 
	//sf::Mouse::setPosition({850, 450});
	//window.setMouseCursorVisible(false);

	glewInit();
	glGetError(); // сброс флага GL_INVALID_ENUM

	sf::Clock clock;
	vector<Model> models;

	Model barrel("barrel/PirateBarrel.fbx");
	models.push_back(barrel);

	Model box("barrel/PirateBox.fbx");
	models.push_back(box); 
	
	Model picachu("pikachu/Pikachu.obj");
	models.push_back(picachu);

	Model plant1("plant/succulent1.obj");
	models.push_back(plant1);
	
	Model plant2("plant/succulent2.obj");
	models.push_back(plant2);


	Init();

	while (window.isOpen())
	{
		sf::Event event;

		while (window.pollEvent(event))
		{

			float camera_speed = 0.5f;

			if (event.type == sf::Event::Closed)
				window.close();
			if (event.type == sf::Event::KeyPressed)
			{
				if (event.key.code == sf::Keyboard::Escape)
				{
					window.close();
					break;
				}
				if (event.key.code == sf::Keyboard::W)
				{
					cameraPos += camera_speed * cameraFront;
				}
				if (event.key.code == sf::Keyboard::S)
				{
					cameraPos -= camera_speed * cameraFront;
				}
				if (event.key.code == sf::Keyboard::A)
				{
					cameraPos -= glm::normalize(glm::cross(cameraFront, cameraUp)) * camera_speed;
				}
				if (event.key.code == sf::Keyboard::D)
				{
					cameraPos += glm::normalize(glm::cross(cameraFront, cameraUp)) * camera_speed;
				}
				if (event.key.code == sf::Keyboard::Q)
				{
					cameraPos += camera_speed * cameraUp;
				}
				if (event.key.code == sf::Keyboard::E)
				{
					cameraPos -= camera_speed * cameraUp;
				}
				if (event.key.code == sf::Keyboard::K)
				{
					conus += 0.5f;
					std::cout << conus << std::endl;
				}
				if (event.key.code == sf::Keyboard::L)
				{
					conus -= 0.5f;
					std::cout << conus << std::endl;
				}
				if (event.key.code == sf::Keyboard::R)
				{
					if (repl == p)
						repl = d;
					else
						repl = p;
				}
				if (event.key.code == sf::Keyboard::Up)
				{
					if (repl == p)
					{
						lightPos -= glm::vec3(0.0f, 0.0f, 0.5f);

						std::cout << lightPos.z << std::endl;
					}
					else
					{
						lightDirection += glm::vec3(0.0f, 0.0f, 0.5f);

						std::cout << lightDirection.z << std::endl;
					}
				}
				if (event.key.code == sf::Keyboard::Down)
				{
					if (repl == p)
					{
						lightPos += glm::vec3(0.0f, 0.0f, 0.5f);

						std::cout << lightPos.z << std::endl;
					}
					else
					{
						lightDirection -= glm::vec3(0.0f, 0.0f, 0.5f);

						std::cout << lightDirection.z << std::endl;
					}
				}
				if (event.key.code == sf::Keyboard::Right)
				{
					if (repl == p)
					{
						lightPos += glm::vec3(0.5f, 0.0f, 0.0f);

						std::cout << lightPos.x << std::endl;
					}
					else
					{
						lightDirection -= glm::vec3(0.5f, 0.0f, 0.0f);

						std::cout << lightDirection.x << std::endl;
					}
				}
				if (event.key.code == sf::Keyboard::Left)
				{
					if (repl == p)
					{
						lightPos -= glm::vec3(0.5f, 0.0f, 0.0f);

						std::cout << lightPos.x << std::endl;
					}
					else
					{
						lightDirection += glm::vec3(0.5f, 0.0f, 0.0f);

						std::cout << lightDirection.x << std::endl;
					}
				}
				if (event.key.code == sf::Keyboard::Comma)
				{
					if (repl == p)
					{
						lightPos += glm::vec3(0.0f, 0.5f, 0.0f);

						std::cout << lightPos.y << std::endl;
					}
					else
					{
						lightDirection -= glm::vec3(0.0f, 0.5f, 0.0f);

						std::cout << lightDirection.y << std::endl;
					}
				}
				if (event.key.code == sf::Keyboard::Period)
				{
					if (repl == p)
					{
						lightPos -= glm::vec3(0.0f, 0.5f, 0.0f);

						std::cout << lightPos.y << std::endl;
					}
					else
					{
						lightDirection += glm::vec3(0.0f, 0.5f, 0.0f);

						std::cout << lightDirection.y << std::endl;
					}
				}
				if (event.key.code == sf::Keyboard::Numpad1)
				{
					sh = rim;

					std::cout << "rim" << std::endl;
				}
				if (event.key.code == sf::Keyboard::Numpad2)
				{
					sh = toon;

					std::cout << "toon" << std::endl;
				}
				if (event.key.code == sf::Keyboard::Numpad3)
				{
					sh = fong;

					std::cout << "fong" << std::endl;
				}
				if (event.key.code == sf::Keyboard::Numpad4)
				{
					type_light = dir;

					std::cout << "dir" << std::endl;
				}
				if (event.key.code == sf::Keyboard::Numpad5)
				{
					type_light = point;

					std::cout << "point" << std::endl;
				}
				if (event.key.code == sf::Keyboard::Numpad6)
				{
					type_light = spot;

					std::cout << "spot" << std::endl;
				}
			}
			if (event.type == sf::Event::MouseMoved)
			{
				float xpos = static_cast<float>(event.mouseMove.x);
				float ypos = static_cast<float>(event.mouseMove.y);

				if (firstMouse)
				{
					last_x = xpos;
					last_y = ypos;
					firstMouse = false;
				}

				float xoffset = xpos - last_x;
				float yoffset = last_y - ypos;
				last_x = xpos;
				last_y = ypos;

				float sensitivity = 0.1f;
				xoffset *= sensitivity;
				yoffset *= sensitivity;

				yaw += xoffset;
				pitch += yoffset;

				if (pitch > 89.0f)
					pitch = 89.0f;
				if (pitch < -89.0f)
					pitch = -89.0f;

				glm::vec3 front;
				front.x = cos(glm::radians(yaw)) * cos(glm::radians(pitch));
				front.y = sin(glm::radians(pitch));
				front.z = sin(glm::radians(yaw)) * cos(glm::radians(pitch));
				cameraFront = glm::normalize(front);
			}
			else if (event.type == sf::Event::Resized)
				glViewport(0, 0, event.size.width, event.size.height);
		}

		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

		GLint shader = ProgramFong;

		if (sh == rim)
			shader = ProgramRim;
		if (sh == toon)
			shader = ProgramToon;
		if (sh == fong)
			shader = ProgramFong;
		
		Draw(clock, models, shader, 1);

		window.display();
	}

	Release();
	return 0;
}