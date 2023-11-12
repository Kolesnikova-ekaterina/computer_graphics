#include <GL/glew.h>
#include <SFML/OpenGL.hpp>
#include <SFML/Window.hpp>
#include <SFML/Graphics.hpp>
#include <iostream>

// 
bool b_fan = true;
bool b_quad = false;
bool b_pent = false;
bool b_simple = true;
bool b_uni = false;
bool b_grad = false;

// ID шейдерной программы
GLuint Program;

// ID атрибута
GLint Attrib_vertex;

// ID Vertex Buffer Object and Vertex Array Object
GLuint VAO_fan, VBO_fan;
GLuint VAO_quad, VBO_quad;
GLuint VAO_pentagon, VBO_pentagon;

struct Vertices 
{
    GLfloat x;
    GLfloat y;
};


// Исходный код вершинного шейдера
const char* VertexShaderSource = R"(
    #version 330 core

    layout (location = 0) in vec2 coord_pos;
    layout (location = 1) in vec3 color_value;

    out vec3 frag_color;

    uniform vec3 color_uni;
    uniform bool simple;
    uniform bool uni;  
    uniform bool grad;
    
    void main() 
    {
        gl_Position = vec4(coord_pos, 0.0, 1.0);

        if(simple)
            frag_color = vec3(0, 0.95f, 0.8f);
        else if(uni)
            frag_color = color_uni;
        else if(grad)
            frag_color = color_value;
    }
    )";

// Исходный код фрагментного шейдера
const char* FragShaderSource = R"(
    #version 330 core
    
    in vec3 frag_color;
    out vec4 color;

    void main() 
    {
        color = vec4(frag_color, 1);
    }
)";


void checkOpenGLerror()
{
    GLenum err = glGetError();
    if (err != GL_NO_ERROR)
    {
        std::cout << "OpenGL error " <<  err << std::endl;
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
    // Создаем вершинный шейдер
    GLuint vShader = glCreateShader(GL_VERTEX_SHADER);
    // Передаем исходный код
    glShaderSource(vShader, 1, &VertexShaderSource, NULL);
    // Компилируем шейдер
    glCompileShader(vShader);
    std::cout << "vertex shader \n";
    // Функция печати лога шейдера
    ShaderLog(vShader); 

    //-----------------------

    // Создаем фрагментный шейдер
    GLuint fShader = glCreateShader(GL_FRAGMENT_SHADER);
    // Передаем исходный код
    glShaderSource(fShader, 1, &FragShaderSource, NULL);
    // Компилируем шейдер
    glCompileShader(fShader);
    std::cout << "fragment shader \n";
    // Функция печати лога шейдера
    ShaderLog(fShader);

    // Создаем программу и прикрепляем шейдеры к ней
    Program = glCreateProgram();
    glAttachShader(Program, vShader);
    glAttachShader(Program, fShader);

    // Линкуем шейдерную программу
    glLinkProgram(Program);
    // Проверяем статус сборки
    int link_ok;
    glGetProgramiv(Program, GL_LINK_STATUS, &link_ok);

    if (!link_ok) 
    {
        std::cout << "error attach shaders \n";
        return;
    }

    checkOpenGLerror();
}


void InitVBO_VAO() 
{
    // Вершины нашего веера
    float fan[] =
    {
         0.0f, -0.5f,         0.4f, 0.4f, 1.0f,

         1.0f, -0.25f,        0.4f, 0.8f, 0.6f,
         0.9f, -0.05f,        0.6f, 0.4f, 0.8f,
         0.75f, 0.07f,        0.4f, 0.8f, 0.6f,
         0.65f, 0.23f,        0.6f, 0.4f, 0.8f,
         0.5f,  0.30f,        0.4f, 0.8f, 0.6f,
         0.35f, 0.42f,        0.6f, 0.4f, 0.8f,
         0.2f,  0.45f,        0.4f, 0.8f, 0.6f,

         0.0f, 0.52f,         0.6f, 0.4f, 0.8f,

        -0.2f, 0.45f,         0.4f, 0.8f, 0.6f,
        -0.35f, 0.42f,        0.6f, 0.4f, 0.8f,
        -0.5f, 0.30f,         0.4f, 0.8f, 0.6f,
        -0.65f, 0.23f,        0.6f, 0.4f, 0.8f,
        -0.75f, 0.07f,        0.4f, 0.8f, 0.6f,
        -0.9f, -0.05f,        0.6f, 0.4f, 0.8f,
        -1.0f, -0.25f,        0.4f, 0.8f, 0.6f
    };

    //объявляем массив атрибутов и буфер
    glGenVertexArrays(1, &VAO_fan);
    glGenBuffers(1, &VBO_fan);
    glBindVertexArray(VAO_fan);

    // Передаем вершины в буфер
    glBindBuffer(GL_ARRAY_BUFFER, VBO_fan);
    glBufferData(GL_ARRAY_BUFFER, sizeof(fan), fan, GL_STATIC_DRAW);

    //вершины четырехугольника
    float quad[] =
    {
        -0.9f, -0.4f,        0.2f, 0.2f, 0.6f,
        -0.5f, 0.4f,         1.0f, 0.6f, 0.4f,
         0.9f, 0.4f,         1.0f, 0.2f, 0.0f,
         0.5f, -0.4f,        0.6f, 1.0f, 0.8f
    };

    //объявляем массив атрибутов и буфер
    glGenVertexArrays(2, &VAO_quad);
    glGenBuffers(2, &VBO_quad);
    glBindVertexArray(VAO_quad);

    // Передаем вершины в буфер
    glBindBuffer(GL_ARRAY_BUFFER, VBO_quad);
    glBufferData(GL_ARRAY_BUFFER, sizeof(quad), quad, GL_STATIC_DRAW);
    
    //вершины пятиугольника
    float pentagon[] =
    {
        -0.5f, -0.7f,        0.0f, 0.53f, 1.0f,
        -0.8f, 0.21f,        0.65f, 1.0f, 0.0f,
         0.0f, 0.8f,         1.0f, 0.8f, 0.0f,
         0.8f, 0.21f,        1.0f, 0.14f, 0.0f,
         0.5f, -0.7f,        0.7f, 0.0f, 1.0f
    };

    //объявляем массив атрибутов и буфер
    glGenVertexArrays(3, &VAO_pentagon);
    glGenBuffers(3, &VBO_pentagon);
    glBindVertexArray(VAO_pentagon);

    // Передаем вершины в буфер
    glBindBuffer(GL_ARRAY_BUFFER, VBO_pentagon);
    glBufferData(GL_ARRAY_BUFFER, sizeof(pentagon), pentagon, GL_STATIC_DRAW);
        
    checkOpenGLerror(); 

}

void Init() 
{
    // Шейдеры
    InitShader();

    // Вершинный буфер
    InitVBO_VAO();
}

void Draw()
{
    glUseProgram(Program); // Устанавливаем шейдерную программу текущей

    //устанавливаем униформы
    glUniform1i(glGetUniformLocation(Program, "simple"), (int)true);
    glUniform1i(glGetUniformLocation(Program, "uni"), (int)false);
    glUniform1i(glGetUniformLocation(Program, "grad"), (int)false);

    
    //смотрим какой тип окрашивания выбран
    if (b_simple)
    {
        glUniform1i(glGetUniformLocation(Program, "simple"), (int)true);
        glUniform1i(glGetUniformLocation(Program, "uni"), (int)false);
        glUniform1i(glGetUniformLocation(Program, "grad"), (int)false);

    }
    else if (b_uni)
    {
        glUniform1i(glGetUniformLocation(Program, "simple"), (int)false);
        glUniform1i(glGetUniformLocation(Program, "uni"), (int)true); 
        glUniform3f(glGetUniformLocation(Program, "color_uni"), 0.95f, 0.0f, 0.8f);
        glUniform1i(glGetUniformLocation(Program, "grad"), (int)false);

    }
    else if (b_grad)
    {
        glUniform1i(glGetUniformLocation(Program, "simple"), (int)false);
        glUniform1i(glGetUniformLocation(Program, "uni"), (int)false);
        glUniform1i(glGetUniformLocation(Program, "grad"), (int)true);

    }
    
    //смотрим какой тип фигуры выбран и подключаем соответственный VBO
    if (b_fan)
    {
        glBindBuffer(GL_ARRAY_BUFFER, VBO_fan);
    }
    else if (b_quad)
    {
        glBindBuffer(GL_ARRAY_BUFFER, VBO_quad);
    }
    else if (b_pent)
    {
        glBindBuffer(GL_ARRAY_BUFFER, VBO_pentagon);
    }
;
    // Подключаем массив аттрибутов с указанием на каких местах кто находится
    glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)0);
    glEnableVertexAttribArray(0);
    glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 5 * sizeof(float), (void*)(2 * sizeof(float)));
    glEnableVertexAttribArray(1); 
    
    glBindBuffer(GL_ARRAY_BUFFER, 0); // Отключаем VBO

    //Рисуем в зависимости от выбранной фигуры
    if (b_fan)
    {
        glDrawArrays(GL_TRIANGLE_FAN, 0, 16);
    } 
    else if (b_quad)
    {
        glDrawArrays(GL_QUADS, 0, 4);
    } 
    else if (b_pent)
    {
        glDrawArrays(GL_POLYGON, 0, 5);
    } 
    
    glUseProgram(0); // Отключаем шейдерную программу
    
    checkOpenGLerror();
}


// Освобождение буфера
void ReleaseVBO_VAO() 
{
    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glDeleteVertexArrays(1, &VAO_fan);
    glDeleteBuffers(1, &VBO_fan);
    glDeleteVertexArrays(2, &VAO_quad);
    glDeleteBuffers(2, &VBO_quad);
    glDeleteVertexArrays(3, &VAO_pentagon);
    glDeleteBuffers(3, &VBO_pentagon);
}

// Освобождение шейдеров
void ReleaseShader() 
{
    // Передавая ноль, мы отключаем шейдерную программу
    glUseProgram(0);
    // Удаляем шейдерную программу
    glDeleteProgram(Program);
}


void Release() 
{
    // Шейдеры
    ReleaseShader();
    // Вершинный буфер
    ReleaseVBO_VAO();
}


int main() 
{
    std::setlocale(LC_ALL, "Russian");

    sf::Window window(sf::VideoMode(950, 950), "Fill", sf::Style::Default, sf::ContextSettings(24));
    window.setVerticalSyncEnabled(true);
    window.setActive(true);
    
    glewInit();
    glGetError(); // сброс флага GL_INVALID_ENUM

    Init();

    while (window.isOpen())
    {
        sf::Event event;

        while (window.pollEvent(event))
        {
            if (event.type == sf::Event::Closed)
                window.close();
            if (event.type == sf::Event::KeyPressed)
            {
                if (event.key.code == sf::Keyboard::Escape)
                {
                    window.close();
                    break;
                }
                else if (event.key.code == sf::Keyboard::A)
                {
                    std::cout << "Плоское окрашивание через констнанту" << std::endl; 
                    b_simple = true;
                    b_uni = false;
                    b_grad = false;

                }
                else if (event.key.code == sf::Keyboard::S)
                {
                    std::cout << "Плоское окрашивание через uniform-переменную" << std::endl;
                    b_simple = false;
                    b_uni = true;
                    b_grad = false;
                }
                else if (event.key.code == sf::Keyboard::D)
                {
                    std::cout << "Градиентное окрашивание" << std::endl;
                    b_simple = false;
                    b_uni = false;
                    b_grad = true;
                }
                else if (event.key.code == sf::Keyboard::Q)
                {
                    std::cout << "Веер" << std::endl;
                    b_fan = true;
                    b_quad = false;
                    b_pent = false;
                }
                else if (event.key.code == sf::Keyboard::W)
                {
                    std::cout << "Четырехугольник" << std::endl;
                    b_fan = false;
                    b_quad = true;
                    b_pent = false;
                }
                else if (event.key.code == sf::Keyboard::E)
                {
                    std::cout << "Пятиугольник" << std::endl;
                    b_fan = false;
                    b_quad = false;
                    b_pent = true;
                }
            }
            else if (event.type == sf::Event::Resized)
                glViewport(0, 0, event.size.width, event.size.height);
        }

        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        Draw();

        window.display();
    }

    Release();
    return 0;
}