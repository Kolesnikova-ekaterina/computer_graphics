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


enum flags_axis {
    OX,
    OY,
    OZ,
    NUL
};

flags_axis ax = NUL;

enum flags_type_progr {
    tetr,
    cube,
    circl
};

flags_type_progr type_prog = tetr;

int sectors = 0;
bool two_tex = false;

float mix_value = 0.4f;

float stepx = 0.0f;
float stepy = 0.0f;
float stepz = -5.0f;

float scalex = 1.0f;
float scaley = 1.0f;
float scalez = 1.0f;

// ID шейдерной программы
GLuint ProgramGrad;
GLuint ProgramTex;


// ID Vertex Buffer Object
GLuint VBO_tetrahedron;
GLuint VBO_cube;
GLuint VBO_circle;

//текстуры
GLuint texture1;
GLuint texture2;



// Исходный код вершинного шейдера
const char* VertexShaderGradColor = R"(
    #version 330 core

    layout (location = 0) in vec3 coord_pos;
    layout (location = 1) in vec3 color_value;

    out vec3 frag_color;

    uniform mat4 model;
    uniform mat4 view; 
    uniform mat4 projection;
    
    void main() 
    {
        gl_Position = projection * view * model * vec4(coord_pos, 1.0);;
        frag_color = color_value;
    }
    )";

const char* VertexShaderTextures = R"(
    #version 330 core

    layout (location = 0) in vec3 coord_pos;
    layout (location = 1) in vec3 color_value;
    layout (location = 2) in vec2 tex_coord_in;

    out vec3 frag_color;
    out vec2 coord_tex;

    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;
    
    void main() 
    { 
        gl_Position = projection * view * model * vec4(coord_pos, 1.0);
        frag_color = color_value;
        coord_tex = tex_coord_in;
    }
    )";

// Исходный код фрагментного шейдера
const char* FragShaderGradColor = R"(
    #version 330 core
    
    in vec3 frag_color;
    in vec2 coord_tex;

    out vec4 color;

    uniform float mix_value;
    
    uniform sampler2D texture1;
    uniform sampler2D texture2;

    void main() 
    {
        color = vec4(frag_color, 1);
    }
)";

const char* FragShaderTextures = R"(
    #version 330 core
    
    in vec3 frag_color;
    in vec2 coord_tex;

    out vec4 color;

    uniform float mix_value;
    
    uniform bool two_tex;

    uniform sampler2D texture1;
    uniform sampler2D texture2;

    void main()  
    {
       if(two_tex)
           color = mix(texture(texture1, coord_tex), texture(texture2, coord_tex), mix_value);
       else
           color = mix(texture(texture1, coord_tex), vec4(frag_color, 1.0), mix_value);
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
    GLuint vShaderGrad = glCreateShader(GL_VERTEX_SHADER);
    // Передаем исходный код
    glShaderSource(vShaderGrad, 1, &VertexShaderGradColor, NULL);
    // Компилируем шейдер
    glCompileShader(vShaderGrad);
    std::cout << "vertex shader g\n";
    // Функция печати лога шейдера
    ShaderLog(vShaderGrad);

    GLuint vShaderTex = glCreateShader(GL_VERTEX_SHADER);
    // Передаем исходный код
    glShaderSource(vShaderTex, 1, &VertexShaderTextures, NULL);
    // Компилируем шейдер
    glCompileShader(vShaderTex);
    std::cout << "vertex shader t\n";
    // Функция печати лога шейдера
    ShaderLog(vShaderTex);

    //-----------------------

    // Создаем фрагментный шейдер
    GLuint fShaderGrad = glCreateShader(GL_FRAGMENT_SHADER);
    // Передаем исходный код
    glShaderSource(fShaderGrad, 1, &FragShaderGradColor, NULL);
    // Компилируем шейдер
    glCompileShader(fShaderGrad);
    std::cout << "fragment shader \n";
    // Функция печати лога шейдера
    ShaderLog(fShaderGrad);

    // Создаем фрагментный шейдер
    GLuint fShaderTex = glCreateShader(GL_FRAGMENT_SHADER);
    // Передаем исходный код
    glShaderSource(fShaderTex, 1, &FragShaderTextures, NULL);
    // Компилируем шейдер
    glCompileShader(fShaderTex);
    std::cout << "fragment shader \n";
    // Функция печати лога шейдера
    ShaderLog(fShaderTex);


    ProgramGrad = glCreateProgram();
    glAttachShader(ProgramGrad, vShaderGrad);
    glAttachShader(ProgramGrad, fShaderGrad);

    // Линкуем шейдерную программу
    glLinkProgram(ProgramGrad);
    // Проверяем статус сборки
    int link_ok;
    glGetProgramiv(ProgramGrad, GL_LINK_STATUS, &link_ok);

    if (!link_ok)
    {
        std::cout << "error attach shaders \n";
        return;
    }

    ProgramTex = glCreateProgram();
    glAttachShader(ProgramTex, vShaderTex);
    glAttachShader(ProgramTex, fShaderTex);

    // Линкуем шейдерную программу
    glLinkProgram(ProgramTex);
    // Проверяем статус сборки
    glGetProgramiv(ProgramTex, GL_LINK_STATUS, &link_ok);

    if (!link_ok)
    {
        std::cout << "error attach shaders \n";
        return;
    }

    checkOpenGLerror();
}


void InitVBO()
{
    //тетраедр

    float a = 2;

    float tetrahedron[] =
    {
        0.0f, 0.0f, 2.0f * sqrt(6) / 4,                       1.0f, 1.0f, 0.0f,
        2.0f / sqrt(3), 0.0f, -2.0f * sqrt(6) / 12,           0.0f, 1.0f, 1.0f,
        -2.0f / sqrt(12), 2.0f / 2, -2.0f * sqrt(6) / 12,     1.0f, 0.0f, 1.0f,

        0.0f, 0.0f, 2.0f * sqrt(6) / 4,                       1.0f, 1.0f, 0.0f,
        -2.0f / sqrt(12),  -2.0f / 2, -2.0f * sqrt(6) / 12,   0.0f, 0.0f, 1.0f,
        2.0f / sqrt(3), 0.0f, -2.0f * sqrt(6) / 12,           0.0f, 1.0f, 1.0f,
    
        0.0f, 0.0f, 2.0f * sqrt(6) / 4,                       1.0f, 1.0f, 0.0f,
        -2.0f / sqrt(12), 2.0f / 2, -2.0f * sqrt(6) / 12,     1.0f, 0.0f, 1.0f,
        -2.0f / sqrt(12),  -2.0f / 2, -2.0f * sqrt(6) / 12,   0.0f, 0.0f, 1.0f,
    
        2.0f / sqrt(3), 0.0f, -2.0f * sqrt(6) / 12,           0.0f, 1.0f, 1.0f,
        -2.0f / sqrt(12),  -2.0f / 2, -2.0f * sqrt(6) / 12,   0.0f, 0.0f, 1.0f,
        -2.0f / sqrt(12), 2.0f / 2, -2.0f * sqrt(6) / 12,     1.0f, 0.0f, 1.0f,

    };

    //объявляем массив атрибутов и буфер
    glGenBuffers(1, &VBO_tetrahedron);

    // передаем вершины в буфер
    glBindBuffer(GL_ARRAY_BUFFER, VBO_tetrahedron);
    glBufferData(GL_ARRAY_BUFFER, sizeof(tetrahedron), tetrahedron, GL_STATIC_DRAW);


    //куб 

    float cube[] = {
        -0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 0.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f,  1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 1.0f,  1.0f, 1.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 1.0f,  1.0f, 1.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 0.0f,

        -0.5f, -0.5f,  0.5f,   0.0f, 1.0f, 1.0f,  0.0f, 0.0f,
         0.5f, -0.5f,  0.5f,   1.0f, 1.0f, 0.0f,  1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,   0.0f, 0.0f, 1.0f,  1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,   0.0f, 0.0f, 1.0f,  1.0f, 1.0f,
        -0.5f,  0.5f,  0.5f,   1.0f, 0.0f, 1.0f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,   0.0f, 1.0f, 1.0f,  0.0f, 0.0f,

        -0.5f,  0.5f,  0.5f,   1.0f, 0.0f, 1.0f,  1.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  1.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,
        -0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,
        -0.5f, -0.5f,  0.5f,   0.0f, 1.0f, 1.0f,  0.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,   1.0f, 0.0f, 1.0f,  1.0f, 0.0f,

         0.5f,  0.5f,  0.5f,   0.0f, 0.0f, 1.0f,  1.0f, 0.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 1.0f,  1.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f,  0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f,  0.0f, 1.0f,
         0.5f, -0.5f,  0.5f,   1.0f, 1.0f, 0.0f,  0.0f, 0.0f,
         0.5f,  0.5f,  0.5f,   0.0f, 0.0f, 1.0f,  1.0f, 0.0f,

        -0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,
         0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f,  1.0f, 1.0f,
         0.5f, -0.5f,  0.5f,   1.0f, 1.0f, 0.0f,  1.0f, 0.0f,
         0.5f, -0.5f,  0.5f,   1.0f, 1.0f, 0.0f,  1.0f, 0.0f,
        -0.5f, -0.5f,  0.5f,   0.0f, 1.0f, 1.0f,  0.0f, 0.0f,
        -0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f,  0.0f, 1.0f,

        -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  0.0f, 1.0f,
         0.5f,  0.5f, -0.5f,  1.0f, 0.0f, 1.0f,  1.0f, 1.0f,
         0.5f,  0.5f,  0.5f,   0.0f, 0.0f, 1.0f,  1.0f, 0.0f,
         0.5f,  0.5f,  0.5f,   0.0f, 0.0f, 1.0f,  1.0f, 0.0f,
        -0.5f,  0.5f,  0.5f,   1.0f, 0.0f, 1.0f,  0.0f, 0.0f,
        -0.5f,  0.5f, -0.5f,  0.0f, 0.0f, 1.0f,  0.0f, 1.0f
    };

    //объявляем массив атрибутов и буфер
    glGenBuffers(1, &VBO_cube);

    // передаем вершины в буфер
    glBindBuffer(GL_ARRAY_BUFFER, VBO_cube);
    glBufferData(GL_ARRAY_BUFFER, sizeof(cube), cube, GL_STATIC_DRAW);


    //кружок

    const int step = 360;
    float angle = 3.14159f * 2.0f / step; 
    float radius = 0.8f; 
    const int vert = step + 2;
    sectors = vert;

    //вершины круга
    float circle[vert * 6];
    
    circle[0] = 0.0f;
    circle[1] = 0.0f;
    circle[2] = 0.0f;
    circle[3] = 1.0f;
    circle[4] = 1.0f;
    circle[5] = 1.0f;

    int range = vert / 3;
    float color_step = 2.0f / range;
    //std::vector<float> colors = {1.0f, 0.0f, 1.0f}; //пастельные тона
    std::vector<float> colors = {1.0f, 0.0f, 0.0f};

    for (int i = 1; i < vert; i++)
    {
        circle[i * 6] = radius * cos(angle * (i - 1));
        circle[(i * 6) + 1] = radius * sin(angle * (i - 1));
        circle[(i * 6) + 2] = 0.0f;
        
        if (i > 0 && i <= range)
        {
            circle[(i * 6) + 3] = colors[0];
            circle[(i * 6) + 4] = colors[1];
            circle[(i * 6) + 5] = colors[2];

            if (colors[1] < 1.0f)
                colors[1] += color_step;
            else
                colors[0] -= color_step;
            /*
            colors[2] -= color_step;
            colors[1] += color_step;
            */
        }
        else if (i > range && i <= range * 2) 
        {
            circle[(i * 6) + 3] = colors[0];
            circle[(i * 6) + 4] = colors[1];
            circle[(i * 6) + 5] = colors[2];
            

            if (colors[2] < 1.0f)
                colors[2] += color_step;
            else
                colors[1] -= color_step;
            /*
            colors[0] -= color_step;
            colors[2] += color_step;*/
        }
        else
        {
            circle[(i * 6) + 3] = colors[0];
            circle[(i * 6) + 4] = colors[1];
            circle[(i * 6) + 5] = colors[2];

            if (colors[0] < 1.0f)
                colors[0] += color_step;
            else
                colors[2] -= color_step;

            /*
            colors[1] -= color_step;
            colors[0] += color_step;*/
        }
    }

    //объявляем массив атрибутов и буфер
    glGenBuffers(1, &VBO_circle);
    
    // Передаем вершины в буфер
    glBindBuffer(GL_ARRAY_BUFFER, VBO_circle);
    glBufferData(GL_ARRAY_BUFFER, sizeof(circle), circle, GL_STATIC_DRAW);

    checkOpenGLerror();

}


void InitTex()
{
    // создаем текстуру
    glGenTextures(1, &texture1);

    // связываем с типом текступы
    glBindTexture(GL_TEXTURE_2D, texture1);

    // настроки отображения текстуры при выходе за пределы диапазона текстуры
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);

    // настройки отображения текстуры в зависимости от удаления или приближения обьекта
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

    // грузим картинку
    int width, height;
    unsigned char* image = SOIL_load_image("cor.png", &width, &height, 0, SOIL_LOAD_RGB);

    //создаем текстуру
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, image);
    SOIL_free_image_data(image);

    //отключаем привязку к текстуре
    glBindTexture(GL_TEXTURE_2D, 0);

    ////////////////////////////////////////////


    // создаем текстуру
    glGenTextures(1, &texture2);

    // связываем с типом текступы
    glBindTexture(GL_TEXTURE_2D, texture2);

    // настроки отображения текстуры при выходе за пределы диапазона текстуры
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);

    // настройки отображения текстуры в зависимости от удаления или приближения обьекта
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

    // грузим картинку
    image = SOIL_load_image("kr.png", &width, &height, 0, SOIL_LOAD_RGB);

    //создаем текстуру
    glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, image);
    SOIL_free_image_data(image);

    //отключаем привязку к текстуре
    glBindTexture(GL_TEXTURE_2D, 0);

}

void Init()
{
    // Шейдеры
    InitShader();

    // Вершинный буфер
    InitVBO();

    //подгрузка текстур
    InitTex();

    //включаем тест глубины
    glEnable(GL_DEPTH_TEST);
}

void Draw(sf::Clock clock)
{
    glm::mat4 view = glm::mat4(1.0f); 
    glm::mat4 projection;
    glm::mat4 model = glm::mat4(1.0f);

    if (type_prog == tetr)
    {
        //тетраедр

        glUseProgram(ProgramGrad); // Устанавливаем шейдерную программу текущей

        model = glm::translate(model, glm::vec3(0.0f, 0.0f, 0.0f));

        float angle = 25.0f;
        //float elapsed1 = clock.getElapsedTime().asSeconds();
        model = glm::rotate(model, glm::radians(angle), glm::vec3(1.0f, 0.0f, 0.0f));
        //model = glm::rotate(model, elapsed1 * glm::radians(10.0f) / 2, glm::vec3(1.0f, 0.0f, 0.0f));

        model = glm::scale(model, glm::vec3(scalex, scaley, scalez));

        view = glm::translate(view, glm::vec3(stepx, stepy, stepz));

        projection = glm::perspective(glm::radians(45.0f), 900.0f / 900.0f, 0.1f, 100.0f);

        glUniformMatrix4fv(glGetUniformLocation(ProgramGrad, "view"), 1, GL_FALSE, glm::value_ptr(view));
        glUniformMatrix4fv(glGetUniformLocation(ProgramGrad, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
        glUniformMatrix4fv(glGetUniformLocation(ProgramGrad, "model"), 1, GL_FALSE, glm::value_ptr(model));

        // подключаем VBO

        glBindBuffer(GL_ARRAY_BUFFER, VBO_tetrahedron);

        // Подключаем массив аттрибутов с указанием на каких местах кто находится
        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)0);
        glEnableVertexAttribArray(0);
        glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)(3 * sizeof(float)));
        glEnableVertexAttribArray(1);

        glBindBuffer(GL_ARRAY_BUFFER, 0); // Отключаем VBO

        //Рисуем 

        glDrawArrays(GL_TRIANGLES, 0, 12);

        // Отключаем массивы атрибутов
        glDisableVertexAttribArray(0);
        glDisableVertexAttribArray(1);


        glUseProgram(0); // Отключаем шейдерную программу

    }
    else if (type_prog == cube)
    {
        //куб(ы)

        glUseProgram(ProgramTex); // Устанавливаем шейдерную программу текущей 

        //связываем текстуры с переменными в шейдере
        glActiveTexture(GL_TEXTURE0);
        glBindTexture(GL_TEXTURE_2D, texture1);
        glUniform1i(glGetUniformLocation(ProgramTex, "texture1"), 0);

        glActiveTexture(GL_TEXTURE1);
        glBindTexture(GL_TEXTURE_2D, texture2);
        glUniform1i(glGetUniformLocation(ProgramTex, "texture2"), 1);
                
        glUniform1f(glGetUniformLocation(ProgramTex, "mix_value"), mix_value);
        glUniform1i(glGetUniformLocation(ProgramTex, "two_tex"), two_tex);


        model = glm::translate(model, glm::vec3(0.0f, 0.0f, 0.0f));

        float angle = 25.0f;
        //float elapsed1 = clock.getElapsedTime().asSeconds();
        model = glm::rotate(model, glm::radians(angle), glm::vec3(1.0f, 1.0f, 0.0f)); 
        //model = glm::rotate(model, elapsed1 * glm::radians(10.0f) / 2, glm::vec3(1.0f, 0.0f, 0.0f));

        model = glm::scale(model, glm::vec3(scalex, scaley, scalez)); 

        view = glm::translate(view, glm::vec3(stepx, stepy, stepz));

        projection = glm::perspective(glm::radians(45.0f), 900.0f / 900.0f, 0.1f, 100.0f);

        glUniformMatrix4fv(glGetUniformLocation(ProgramTex, "view"), 1, GL_FALSE, glm::value_ptr(view));
        glUniformMatrix4fv(glGetUniformLocation(ProgramTex, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
        glUniformMatrix4fv(glGetUniformLocation(ProgramTex, "model"), 1, GL_FALSE, glm::value_ptr(model));


        // подключаем VBO
        glBindBuffer(GL_ARRAY_BUFFER, VBO_cube);

        // Подключаем массив аттрибутов с указанием на каких местах кто находится
        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)0);
        glEnableVertexAttribArray(0);

        glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(3 * sizeof(float)));
        glEnableVertexAttribArray(1);

        glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, 8 * sizeof(float), (void*)(6 * sizeof(float)));
        glEnableVertexAttribArray(2); 

        glBindBuffer(GL_ARRAY_BUFFER, 0);  // Отключаем VBO

        //Рисуем 
        glDrawArrays(GL_TRIANGLES, 0, 36);

        // Отключаем массивы атрибутов
        glDisableVertexAttribArray(0);
        glDisableVertexAttribArray(1);
        glDisableVertexAttribArray(2);

        glUseProgram(0); // Отключаем шейдерную программу

    }
    else if(type_prog == circl)
    {
        //кружок

        glUseProgram(ProgramGrad); // Устанавливаем шейдерную программу текущей

        model = glm::translate(model, glm::vec3(0.0f, 0.0f, 0.0f));

        float angle = 60.0f;
        //float elapsed1 = clock.getElapsedTime().asSeconds();
        model = glm::rotate(model, glm::radians(angle), glm::vec3(-1.0f, 0.0f, 0.0f));
        //model = glm::rotate(model, elapsed1 * glm::radians(10.0f) / 2, glm::vec3(1.0f, 0.0f, 0.0f));

        model = glm::scale(model, glm::vec3(scalex, scaley, scalez));

        view = glm::translate(view, glm::vec3(stepx, stepy, stepz));

        projection = glm::perspective(glm::radians(45.0f), 900.0f / 900.0f, 0.1f, 100.0f);

        glUniformMatrix4fv(glGetUniformLocation(ProgramGrad, "view"), 1, GL_FALSE, glm::value_ptr(view));
        glUniformMatrix4fv(glGetUniformLocation(ProgramGrad, "projection"), 1, GL_FALSE, glm::value_ptr(projection));
        glUniformMatrix4fv(glGetUniformLocation(ProgramGrad, "model"), 1, GL_FALSE, glm::value_ptr(model));

        //подключаем VBO
        glBindBuffer(GL_ARRAY_BUFFER, VBO_circle);

        // Подключаем массив аттрибутов с указанием на каких местах кто находится
        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)0);
        glEnableVertexAttribArray(0);
        glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 6 * sizeof(float), (void*)(3 * sizeof(float)));
        glEnableVertexAttribArray(1);

        glBindBuffer(GL_ARRAY_BUFFER, 0); // Отключаем VBO

        //Рисуем в зависимости от выбранной фигуры
        glDrawArrays(GL_TRIANGLE_FAN, 0, sectors);

        // Отключаем массивы атрибутов
        glDisableVertexAttribArray(0);
        glDisableVertexAttribArray(1);


        glUseProgram(0); // Отключаем шейдерную программу

    }

    checkOpenGLerror();
}


// Освобождение буфера
void ReleaseVBO()
{
    glBindBuffer(GL_ARRAY_BUFFER, 0);
    glDeleteBuffers(1, &VBO_tetrahedron);
    glDeleteBuffers(1, &VBO_cube);
    glDeleteBuffers(1, &VBO_circle);
}

// Освобождение шейдеров
void ReleaseShader()
{
    // Передавая ноль, мы отключаем шейдерную программу
    glUseProgram(0);
    // Удаляем шейдерные программы
    glDeleteProgram(ProgramGrad);
    glDeleteProgram(ProgramTex);
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

    glewInit();
    glGetError(); // сброс флага GL_INVALID_ENUM

    Init();

    sf::Clock clock; 
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
                else if (event.key.code == sf::Keyboard::Up && type_prog == cube)
                {
                    mix_value += 0.01f; // change this value accordingly (might be too slow or too fast based on system hardware)
                    if (mix_value >= 1.0f)
                        mix_value = 1.0f;

                    std::cout << mix_value << std::endl;
                }

                else if (event.key.code == sf::Keyboard::Down && type_prog == cube)
                {
                    mix_value -= 0.01f; // change this value accordingly (might be too slow or too fast based on system hardware)
                    if (mix_value <= 0.0f)
                        mix_value = 0.0f;

                    std::cout << mix_value << std::endl;
                }
                else if (event.key.code == sf::Keyboard::Right)// && b_tetr)
                {
                    if (ax == OX)
                    {
                        stepx += 0.1f; // change this value accordingly (might be too slow or too fast based on system hardware)
                        
                    }
                    else if (ax == OY)
                    {
                        stepy += 0.1f; // change this value accordingly (might be too slow or too fast based on system hardware)
                        
                    }
                    else if (ax == OZ)
                    {
                        stepz += 0.5f; // change this value accordingly (might be too slow or too fast based on system hardware)
                        std::cout << stepz << std::endl;
                    }
                }

                else if (event.key.code == sf::Keyboard::Left)// && b_tetr)
                {
                    if (ax == OX)
                    {
                        stepx -= 0.1f; // change this value accordingly (might be too slow or too fast based on system hardware)
                        
                    }
                    else if (ax == OY)
                    {
                        stepy -= 0.1f; // change this value accordingly (might be too slow or too fast based on system hardware)
                        
                    }
                    else if (ax == OZ)
                    {
                        stepz -= 0.5f; // change this value accordingly (might be too slow or too fast based on system hardware)
                        std::cout << stepz << std::endl;
                    }
                }
                else if (event.key.code == sf::Keyboard::X)
                {
                    std::cout << "OX" << std::endl;
                    ax = OX;

                }
                else if (event.key.code == sf::Keyboard::Y)
                {
                    std::cout << "OY" << std::endl;
                    ax = OY;
                }
                else if (event.key.code == sf::Keyboard::Z)
                {
                    std::cout << "OZ" << std::endl;
                    ax = OZ;
                }
                else if (event.key.code == sf::Keyboard::C)
                {
                    std::cout << "кубибки" << std::endl;
                    type_prog = cube;
                }
                else if (event.key.code == sf::Keyboard::T)
                {
                    std::cout << "тетраедр" << std::endl;
                    type_prog = tetr; 
                }
                else if (event.key.code == sf::Keyboard::O) 
                {
                    std::cout << "кружок" << std::endl;
                    type_prog = circl; 
                }
                else if (event.key.code == sf::Keyboard::Numpad1) // && type_prog == circl)
                {
                    std::cout << "сжатие по OX" << std::endl;
                    scalex -= 0.1f;
                }
                else if (event.key.code == sf::Keyboard::Numpad2) // && type_prog == circl)
                {
                    std::cout << "растяжение по OX" << std::endl;
                    scalex += 0.1f;
                }
                else if (event.key.code == sf::Keyboard::Numpad4) // && type_prog == circl)
                { 
                    std::cout << "сжатие по OY" << std::endl;
                    scaley -= 0.1f;
                }
                else if (event.key.code == sf::Keyboard::Numpad5) // && type_prog == circl)
                {
                    std::cout << "растяжение по OY" << std::endl;
                    scaley += 0.1f;
                }
                else if (event.key.code == sf::Keyboard::Numpad7) // && type_prog == circl)
                {
                    std::cout << "сжатие по OZ" << std::endl;
                    scalez -= 0.1f;
                }
                else if (event.key.code == sf::Keyboard::Numpad8) // && type_prog == circl)
                {
                    std::cout << "растяжение по OZ" << std::endl;
                    scalez += 0.1f;
                }
                else if (event.key.code == sf::Keyboard::D && type_prog == cube)
                {
                    two_tex = !two_tex;
                }
                
            }
            else if (event.type == sf::Event::Resized)
                glViewport(0, 0, event.size.width, event.size.height);
        }

        glClear(GL_COLOR_BUFFER_BIT);
        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

        Draw(clock);

        window.display();
    }

    Release();
    return 0;
}