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

enum flags_type_model {
	tree,
	box1,
	box2
};



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
GLuint ProgramTree;
GLuint ProgramBox;


// ID Vertex Buffer Object
GLuint VBO_instance_box1;
GLuint VBO_instance_box2;

//инстансинг
GLuint quantity = 5
;
glm::mat4* modelMatrices;
glm::mat4* modelMatricesToPosition;
glm::mat4* modelMatricesToCenter;
glm::mat4* localRotateMatrices;
glm::mat4* tmpModelMatrices;
// Исходный код вершинного шейдера

const char* VertexShaderTree = R"(
    #version 330 core

    layout (location = 0) in vec3 coord_pos;
    layout (location = 2) in vec2 tex_coord_in;

    out vec2 coord_tex;

    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;
    
    void main() 
    { 
        gl_Position = projection * view * model * vec4(coord_pos, 1.0);
        coord_tex = tex_coord_in;
        //coord_tex = vec2(tex_coord_in.x, 1.0f - tex_coord_in.y); //если текстуры ннеправильно наложились
    }
    )";


const char* VertexShaderBox = R"(
    #version 330 core

    layout (location = 0) in vec3 coord_pos;
    layout (location = 2) in vec2 tex_coord_in;


    out vec2 coord_tex;

    uniform mat4 instanceModel[15];
    uniform mat4 view;
    uniform mat4 projection;
    
    void main() 
    { 
        gl_Position = projection * view  * instanceModel[gl_InstanceID] * vec4(coord_pos, 1.0);
        coord_tex = tex_coord_in;
    }
    )";

// Исходный код фрагментного шейдера

const char* FragShaderTree = R"(
    #version 330 core
    
    in vec2 coord_tex;

    uniform sampler2D texture_diffuse1;

    void main()  
    {
       gl_FragColor = texture(texture_diffuse1, coord_tex);
        //gl_FragColor = vec4(1.0,0.0,0.0,1.0);
    } 
)";


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
	GLuint vShaderTree = glCreateShader(GL_VERTEX_SHADER);
	// Передаем исходный код
	glShaderSource(vShaderTree, 1, &VertexShaderTree, NULL);
	// Компилируем шейдер
	glCompileShader(vShaderTree);
	std::cout << "vertex shader t\n";
	// Функция печати лога шейдера
	ShaderLog(vShaderTree);

	GLuint vShaderBox = glCreateShader(GL_VERTEX_SHADER);
	// Передаем исходный код
	glShaderSource(vShaderBox, 1, &VertexShaderBox, NULL);
	// Компилируем шейдер
	glCompileShader(vShaderBox);
	std::cout << "vertex shader t\n";
	// Функция печати лога шейдера
	ShaderLog(vShaderBox);

	//-----------------------

	// Создаем фрагментный шейдер
	GLuint fShaderTree = glCreateShader(GL_FRAGMENT_SHADER);
	// Передаем исходный код
	glShaderSource(fShaderTree, 1, &FragShaderTree, NULL);
	// Компилируем шейдер
	glCompileShader(fShaderTree);
	std::cout << "fragment shader \n";
	// Функция печати лога шейдера
	ShaderLog(fShaderTree);

	ProgramTree = glCreateProgram();
	glAttachShader(ProgramTree, vShaderTree);
	glAttachShader(ProgramTree, fShaderTree);

	// Линкуем шейдерную программу
	glLinkProgram(ProgramTree);

	// Проверяем статус сборки
	int link_ok;
	glGetProgramiv(ProgramTree, GL_LINK_STATUS, &link_ok);

	if (!link_ok)
	{
		std::cout << "error attach shaders \n";
		return;
	}

	ProgramBox = glCreateProgram();
	glAttachShader(ProgramBox, vShaderBox);
	glAttachShader(ProgramBox, fShaderTree);

	// Линкуем шейдерную программу
	glLinkProgram(ProgramBox);

	// Проверяем статус сборки
	glGetProgramiv(ProgramBox, GL_LINK_STATUS, &link_ok);

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



void Draw(sf::Clock clock, Model modelka, flags_type_model type_model, int count)
{

	glm::mat4 view = glm::mat4(1.0f);
	glm::mat4 model = glm::mat4(1.0f);

	if (type_model == tree)
	{
		//тетраедр

		glUseProgram(ProgramTree); // Устанавливаем шейдерную программу текущей


		float angle = -25.0f;

		model = glm::scale(model, glm::vec3(0.0025f, 0.0025f, 0.0025f));

		model = glm::translate(model, glm::vec3(0.0f, 15, 0.0f));

		model = glm::rotate(model, clock.getElapsedTime().asSeconds() * glm::radians(angle), glm::vec3(0.0f, 1.0f, 0.0f));

		view = glm::lookAt(cameraPos, cameraPos + cameraFront, cameraUp);

		projection = glm::perspective(glm::radians(45.0f), 900.0f / 900.0f, 0.1f, 100.0f);

		glUniformMatrix4fv(glGetUniformLocation(ProgramTree, "view"), 1, GL_FALSE, glm::value_ptr(view));
		glUniformMatrix4fv(glGetUniformLocation(ProgramTree, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
		glUniformMatrix4fv(glGetUniformLocation(ProgramTree, "model"), 1, GL_FALSE, glm::value_ptr(model));

		modelka.Draw(ProgramTree, count);

		glUseProgram(0); // Отключаем шейдерную программу

	}
	if (type_model == box1)
	{

		glUseProgram(ProgramBox); // Устанавливаем шейдерную программу текущей


		float angle = 25.0f;
		for (int i = 0; i < quantity; i++)
		{
			model = glm::rotate(glm::mat4(1.0f), clock.getElapsedTime().asSeconds() * glm::radians(90.0f), glm::vec3(0.0f, 0.0f, 1.0f));
			tmpModelMatrices[i] = modelMatrices[i] * modelMatricesToCenter[i] * localRotateMatrices[i] * glm::rotate(glm::mat4(1.0f), clock.getElapsedTime().asSeconds() * glm::radians(20.0f), glm::vec3(0.0f, 1.0f, 0.0f)) * modelMatricesToPosition[i] * model;
		}
		view = glm::lookAt(cameraPos, cameraPos + cameraFront, cameraUp);


		glUniformMatrix4fv(glGetUniformLocation(ProgramBox, "view"), 1, GL_FALSE, glm::value_ptr(view));
		glUniformMatrix4fv(glGetUniformLocation(ProgramBox, "projection"), 1, GL_FALSE, glm::value_ptr(projection));


		glUniformMatrix4fv(glGetUniformLocation(ProgramBox, "instanceModel"), quantity, GL_FALSE, &tmpModelMatrices[0][0][0]);

		modelka.Draw(ProgramBox, count);

		glUseProgram(0); // Отключаем шейдерную программу

	}
	if (type_model == box2)
	{

		glUseProgram(ProgramBox); // Устанавливаем шейдерную программу текущей


		float angle = 25.0f;
		for (int i = 0; i < quantity; i++)
		{
			model = glm::rotate(glm::mat4(1.0f), clock.getElapsedTime().asSeconds() * glm::radians(90.0f), glm::vec3(0.0f, 0.0f, 1.0f));
			tmpModelMatrices[i] = modelMatrices[i] * modelMatricesToCenter[i] * localRotateMatrices[i] * glm::rotate(glm::mat4(1.0f), clock.getElapsedTime().asSeconds() * glm::radians(20.0f), glm::vec3(0.0f, 1.0f, 0.0f)) * modelMatricesToPosition[i] * model;
			tmpModelMatrices[i] = glm::translate(tmpModelMatrices[i], glm::vec3(2.0f, 0.0f, 0.0f));
		}
		view = glm::lookAt(cameraPos, cameraPos + cameraFront, cameraUp);

		

		glUniformMatrix4fv(glGetUniformLocation(ProgramBox, "view"), 1, GL_FALSE, glm::value_ptr(view));
		glUniformMatrix4fv(glGetUniformLocation(ProgramBox, "projection"), 1, GL_FALSE, glm::value_ptr(projection));


		glUniformMatrix4fv(glGetUniformLocation(ProgramBox, "instanceModel"), quantity, GL_FALSE, &tmpModelMatrices[0][0][0]);

		modelka.Draw(ProgramBox, count);

		glUseProgram(0); // Отключаем шейдерную программу

	}

	checkOpenGLerror();
}


// Освобождение буфера
void ReleaseVBO()
{
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	glDeleteBuffers(1, &VBO_instance_box1);
	glDeleteBuffers(1, &VBO_instance_box2);
}

// Освобождение шейдеров
void ReleaseShader()
{
	// Передавая ноль, мы отключаем шейдерную программу
	glUseProgram(0);
	// Удаляем шейдерные программы
	glDeleteProgram(ProgramBox);
	glDeleteProgram(ProgramTree);
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
	//window.setMouseCursorVisible(false);

	glewInit();
	glGetError(); // сброс флага GL_INVALID_ENUM

	sf::Clock clock;

	Model ourModel("chrtree/tree.obj");
	Model box1_model("box1/b1.obj");
	Model box2_model("box2/b2.obj");

	Init();

	modelMatrices = new glm::mat4[quantity];
	localRotateMatrices = new glm::mat4[quantity];
	modelMatricesToPosition = new glm::mat4[quantity];
	modelMatricesToCenter = new glm::mat4[quantity];
	tmpModelMatrices = new glm::mat4[quantity];

	srand(static_cast<unsigned int>(clock.getElapsedTime().asSeconds()));
	float radius = 10.0;
	float offset = 4.0f;

	for (unsigned int i = 0; i < quantity; i++)
	{
		glm::mat4 model = glm::mat4(1.0f);
		// перемещаем по осям
		float angle = (float)i / (float)quantity * 360.0f;

		float displacement = (rand() % (int)(offset * 100)) / 100.0f ;
		float x = sin(angle) * radius + displacement;

		displacement = (rand() % (int)(offset * 100)) / 100.0f - offset;
		float y = (displacement + 10.0f); 

		float z = cos(angle) * radius + displacement;

		modelMatricesToPosition[i] = glm::translate(glm::mat4(1.0f), glm::vec3(x, y, z) * 10.0f);
		modelMatricesToCenter[i] = glm::translate(glm::mat4(1.0f), glm::vec3(-x, -y, -z) * 10.0f);
		model = glm::translate(model, glm::vec3(0.0f, 3.0f, 0.0f));

		// размер
		float scale = static_cast<float>((rand() % 40) / 100000.0 + 0.0015);
		model = glm::scale(model, glm::vec3(scale));
		//model = glm::scale(model, glm::vec3(0.0025f, 0.0025f, 0.0025f));
		
		//поворот
		float rotAngle = static_cast<float>((rand() % 360));
		//model = glm::rotate(model, rotAngle, glm::vec3(0.4f, 0.6f, 0.8f));

		localRotateMatrices[i] = glm::rotate(glm::mat4(1.0f), (float)(rand() % 20), glm::vec3(1.0f, 5.0f, 0.0f));

		modelMatrices[i] = model * modelMatricesToPosition[i];
	}




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

		Draw(clock, ourModel, tree, 1);
		Draw(clock, box1_model, box1, quantity);

		window.display();
	}

	Release();
	return 0;
}